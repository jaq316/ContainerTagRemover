using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ContainerTagRemover.Interfaces;

namespace ContainerTagRemover.Services
{
    public class AzureContainerRegistryClient(HttpClient httpClient) : IContainerRegistryClient
    {
        public async Task AuthenticateAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<string>> ListTagsAsync(string repository)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://{repository}.azurecr.io/v2/_catalog");
            var response = await httpClient.SendAsync(request);

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
            var response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Failed to delete tag {tag} for repository {repository}: {response.ReasonPhrase}");
            }
        }

        private static IEnumerable<string> ParseTags(string content)
        {
            using (JsonDocument document = JsonDocument.Parse(content))
            {
                JsonElement root = document.RootElement;
                JsonElement tagsElement = root.GetProperty("tags");
                var tags = new List<string>();

                foreach (JsonElement tagElement in tagsElement.EnumerateArray())
                {
                    tags.Add(tagElement.GetString());
                }

                return tags;
            }
        }
    }
}
