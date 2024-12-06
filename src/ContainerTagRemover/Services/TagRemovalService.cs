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
    public class TagRemovalService
    {

        private readonly IContainerRegistryClient _registryClient;
        private readonly TagRemovalConfig _config;

        public TagRemovalService(IContainerRegistryClient registryClient, TagRemovalConfig config)
        {
            _registryClient = registryClient;
            _config = config;
        }

        public async Task RemoveOldTagsAsync(string image)
        {
            var tags = await _registryClient.ListTagsAsync(image);
            var tagsToRemove = DetermineTagsToRemove(tags);

            foreach (var tag in tagsToRemove)
            {
                await _registryClient.DeleteTagAsync(image, tag);
            }
        }

        public IEnumerable<string> DetermineTagsToRemove(IEnumerable<Tag> tags)
        {
            var semverTags = tags
                .Select(tag => new { Version = SemVersion.TryParse(tag.Name, out var version) ? version : null, tag.Digest })
                .Where(version => version != null)
                .OrderByDescending(tag => tag.Version, new VersionComparer())
                .GroupBy(tag => tag.Digest)
                .Select(tg => tg.Select(tag => tag.Version).First())
                .ToList();

            var tagsToKeep = new HashSet<SemVersion>();

            KeepLatestVersions(semverTags, tagsToKeep, _config.Major, v => v.Major);
            KeepLatestVersions(semverTags, tagsToKeep, _config.Minor, v => (v.Major, v.Minor));

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
    }
}
