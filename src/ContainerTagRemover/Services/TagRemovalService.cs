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

            KeepLatestVersions(semverTags, tagsToKeep, config.Major, v => v.Major);
            KeepLatestVersions(semverTags, tagsToKeep, config.Minor, v => (v.Major, v.Minor));

            return semverTags.Where(v => !tagsToKeep.Any(t => t.ToString() == v.ToString())).Select(v => v.ToString());
        }

        private static void KeepLatestVersions<T>(List<SemVersion> semverTags, HashSet<SemVersion> tagsToKeep, int count, Func<SemVersion, T> keySelector)
        {
            var groupedTags = semverTags.GroupBy(keySelector);

            foreach (var group in groupedTags)
            {
                tagsToKeep.UnionWith(group.Take(count));
            }
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
