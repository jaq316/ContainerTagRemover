using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContainerTagRemover.Configuration;
using ContainerTagRemover.Interfaces;
using Semver;

namespace ContainerTagRemover.Services
{
    public class TagRemovalService
    {
        private readonly IContainerRegistryClient _registryClient;
        private readonly TagRemovalConfig _config;

        public TagRemovalService(IContainerRegistryClient registryClient, TagRemovalConfig config)
        {
            _registryClient = registryClient;
            _config = config;
        }

        public async Task RemoveOldTagsAsync(string repository)
        {
            var tags = await _registryClient.ListTagsAsync(repository);
            var tagsToRemove = DetermineTagsToRemove(tags);

            foreach (var tag in tagsToRemove)
            {
                await _registryClient.DeleteTagAsync(repository, tag);
            }
        }

        public IEnumerable<string> DetermineTagsToRemove(IEnumerable<string> tags)
        {
            var semverTags = tags
                .Select(tag => SemVersion.TryParse(tag, out var version) ? version : null)
                .Where(version => version != null)
                .OrderByDescending(version => version)
                .ToList();

            var tagsToKeep = new HashSet<SemVersion>();

            KeepLatestVersions(semverTags, tagsToKeep, _config.Major, v => v.Major);
            KeepLatestVersions(semverTags, tagsToKeep, _config.Minor, v => (v.Major, v.Minor));
            KeepLatestVersions(semverTags, tagsToKeep, _config.Patch, v => (v.Major, v.Minor, v.Patch));

            return semverTags.Except(tagsToKeep).Select(v => v.ToString());
        }

        private void KeepLatestVersions<T>(List<SemVersion> semverTags, HashSet<SemVersion> tagsToKeep, int count, Func<SemVersion, T> keySelector)
        {
            var groupedTags = semverTags.GroupBy(keySelector);

            foreach (var group in groupedTags)
            {
                tagsToKeep.UnionWith(group.Take(count));
            }
        }
    }
}
