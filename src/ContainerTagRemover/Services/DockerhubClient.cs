using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ContainerTagRemover.Interfaces;
using Newtonsoft.Json.Linq;

namespace ContainerTagRemover.Services
{
    public class DockerhubClient : IContainerRegistryClient
    {
        private readonly HttpClient _httpClient;

        public DockerhubClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task AuthenticateAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<string>> ListTagsAsync(string repository)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://hub.docker.com/v2/repositories/{repository}/tags");
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
            var request = new HttpRequestMessage(HttpMethod.Delete, $"https://hub.docker.com/v2/repositories/{repository}/tags/{tag}");
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Failed to delete tag {tag} for repository {repository}: {response.ReasonPhrase}");
            }
        }

        private static IEnumerable<string> ParseTags(string content)
        {
            var json = JObject.Parse(content);
            var tags = json["tags"].ToObject<List<string>>();
            return tags;
        }
    }
}
