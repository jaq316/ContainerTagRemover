using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Containers.ContainerRegistry;
using Azure.Identity;
using ContainerTagRemover.Interfaces;

namespace ContainerTagRemover.Services
{
    public class AzureContainerRegistryClient : IContainerRegistryClient
    {
        private readonly ContainerRegistryClient _client;

        public AzureContainerRegistryClient(string registryUrl)
        {
            _client = new ContainerRegistryClient(new Uri(registryUrl), new DefaultAzureCredential(includeInteractiveCredentials: true),
                new ContainerRegistryClientOptions()
                {
                    Audience = ContainerRegistryAudience.AzureResourceManagerPublicCloud
                });
        }

        public Task AuthenticateAsync(CancellationToken cancellationToken = default)
        {
            // No explicit authentication needed as DefaultAzureCredential handles it
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<Tag>> ListTagsAsync(string image)
        {
            var tags = new List<Tag>();
            var repository = _client.GetRepository(image);
            foreach (var item in repository.GetAllManifestProperties(ArtifactManifestOrder.LastUpdatedOnDescending))
            {
                tags.AddRange(item.Tags.Select(t => new Tag(t, item.Digest)));
            }
            return tags;
        }

        public async Task DeleteTagAsync(string image, string tag)
        {
            RegistryArtifact tagArtifact = _client.GetArtifact(image, tag);
            _ = await tagArtifact.DeleteTagAsync(tag);
        }
    }
}
