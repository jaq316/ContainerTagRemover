using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ContainerTagRemover.Interfaces;

namespace ContainerTagRemover.Services
{
    public class AzureContainerRegistryClient : IContainerRegistryClient, IAuthenticationClient
    {
        private readonly IAuthenticationClient _authenticationClient;
        private readonly HttpClient _httpClient;

        public AzureContainerRegistryClient(IAuthenticationClient authenticationClient, HttpClient httpClient)
        {
            _authenticationClient = authenticationClient;
            _httpClient = httpClient;
        }

        public async Task AuthenticateAsync()
        {
            await _authenticationClient.AuthenticateAsync();
        }

        public async Task<IEnumerable<string>> ListTagsAsync(string repository)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://{repository}.azurecr.io/v2/_catalog");
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Failed to list tags for repository {repository}: {response.ReasonPhrase}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var tags = ParseTags(content);
            return tags;
        }

        public async Task DeleteTagAsync(string repository, string tag)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"https://{repository}.azurecr.io/v2/{repository}/manifests/{tag}");
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Failed to delete tag {tag} for repository {repository}: {response.ReasonPhrase}");
            }
        }

        private IEnumerable<string> ParseTags(string content)
        {
            // Implement tag parsing logic here
            throw new NotImplementedException();
        }
    }
}
