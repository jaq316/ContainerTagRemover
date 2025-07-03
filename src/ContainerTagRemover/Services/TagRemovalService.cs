using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContainerTagRemover.Configuration;
using ContainerTagRemover.Interfaces;
using Semver;

namespace ContainerTagRemover.Services
{
    public class VersionComparer : IComparer<SemVersion>
    {
        public int Compare(SemVersion x, SemVersion y)
        {
            return x.CompareSortOrderTo(y);
        }
    }
    public class TagRemovalService(IContainerRegistryClient registryClient, TagRemovalConfig config)
    {
        private readonly List<string> removedTags = new List<string>();
        private readonly List<string> keptTags = new List<string>();

        public virtual async Task RemoveOldTagsAsync(string image)
        {
            var tags = await registryClient.ListTagsAsync(image);
            var tagsToRemove = DetermineTagsToRemove(tags.Select(t => t.Name));

            foreach (var tag in tagsToRemove)
            {
                await registryClient.DeleteTagAsync(image, tag);
                removedTags.Add(tag);
            }

            var tagsToKeep = tags.Select(t => t.Name).Except(tagsToRemove);
            keptTags.AddRange(tagsToKeep);
        }

        public IEnumerable<string> DetermineTagsToRemove(IEnumerable<string> tags)
        {
            var semverTags = tags
                .Select(tag => SemVersion.TryParse(tag, out var version) ? version : null)
                .Where(version => version != null)
                .OrderByDescending(version => version, new VersionComparer())
                .ToList();

            var tagsToKeep = new HashSet<SemVersion>();

            // First filter the major versions to only include the latest number of major versions as configured
            var latestMajorVersions = semverTags
                .GroupBy(v => v.Major)
                .OrderByDescending(g => g.Key)
                .Take(config.Major)
                .SelectMany(g => g)
                .ToList();

            // Then filter out only the latest number of minor versions for each major version
            var latestMinorVersions = latestMajorVersions
                .GroupBy(v => v.Major)
                .Select(g => new { Major = g.Key, Versions = g.Select(v => v) })
                .Select(g => g.Versions.OrderByDescending(v => v, new VersionComparer()).Take(config.Minor))
                .SelectMany(v => v)
                .ToList();

            tagsToKeep.UnionWith(latestMinorVersions);

            // Add explicitly kept tags from configuration
            var explicitlyKeptTags = config.KeepTags
                .Select(tag => SemVersion.TryParse(tag, out var version) ? version : null)
                .Where(version => version != null);
            tagsToKeep.UnionWith(explicitlyKeptTags);

            // Also keep non-semver tags that are explicitly specified
            var nonSemverKeptTags = config.KeepTags
                .Where(tag => !SemVersion.TryParse(tag, out _))
                .ToHashSet();

            var tagsToRemove = new List<string>();

            // Build a HashSet of kept tag strings for O(1) lookups
            var keepStrings = tagsToKeep.Select(t => t.ToString()).ToHashSet();

            // Add semver tags that should be removed
            tagsToRemove.AddRange(semverTags.Where(v => !keepStrings.Contains(v.ToString())).Select(v => v.ToString()));

            // Add non-semver tags that should be removed (not in the keep list)
            var nonSemverTags = tags.Where(tag => !SemVersion.TryParse(tag, out _));
            tagsToRemove.AddRange(nonSemverTags.Where(tag => !nonSemverKeptTags.Contains(tag)));

            return tagsToRemove;
        }

        public virtual List<string> GetRemovedTags()
        {
            return removedTags;
        }

        public virtual List<string> GetKeptTags()
        {
            return keptTags;
        }
    }
}
