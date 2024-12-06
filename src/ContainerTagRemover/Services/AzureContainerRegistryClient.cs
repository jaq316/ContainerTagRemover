using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure;
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
            _client = new ContainerRegistryClient(new Uri(registryUrl), new DefaultAzureCredential());
        }

        public Task AuthenticateAsync(CancellationToken cancellationToken = default)
        {
            // No explicit authentication needed as DefaultAzureCredential handles it
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<string>> ListTagsAsync(string image)
        {
            var tags = new List<string>();
            var repository = _client.GetRepository(image);
            await foreach (var tag in repository.GetTagsAsync())
            {
                tags.Add(tag.Name);
            }
            return tags;
        }

        public async Task DeleteTagAsync(string image, string tag)
        {
            var repository = _client.GetRepository(image);
            await repository.DeleteTagAsync(tag);
        }
    }
}
