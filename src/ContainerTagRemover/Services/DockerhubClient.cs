using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ContainerTagRemover.Interfaces;
using System.Threading;

namespace ContainerTagRemover.Services
{
    public class DockerhubClient : IContainerRegistryClient
    {
        private readonly HttpClient _httpClient;
        private readonly string registryUrl;
        private string _token;

        public DockerhubClient(HttpClient httpClient, string registryUrl)
        {
            _httpClient = httpClient;
            this.registryUrl = registryUrl;
        }

        public async Task AuthenticateAsync(CancellationToken cancellationToken = default)
        {
            string _username = Environment.GetEnvironmentVariable("DOCKERHUB_USERNAME");
            string _password = Environment.GetEnvironmentVariable("DOCKERHUB_PASSWORD");

            if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
            {
                throw new InvalidOperationException("Environment variables DOCKERHUB_USERNAME and DOCKERHUB_PASSWORD must be set.");
            }

            var request = new HttpRequestMessage(HttpMethod.Post, "https://hub.docker.com/v2/users/login/");
            var content = new StringContent(JsonSerializer.Serialize(new { username = _username, password = _password }), System.Text.Encoding.UTF8, "application/json");
            request.Content = content;

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Failed to authenticate with DockerHub: {response.ReasonPhrase}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(responseContent);
            _token = jsonDocument.RootElement.GetProperty("token").GetString();
        }

        public async Task<IEnumerable<Tag>> ListTagsAsync(string image)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://hub.docker.com/v2/repositories/{registryUrl}/{image}/tags");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Failed to list tags for image {image}: {response.ReasonPhrase}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var tags = ParseTags(content);
            return tags;
        }

        public async Task DeleteTagAsync(string image, string tag)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"https://hub.docker.com/v2/repositories/{registryUrl}/{image}/tags/{tag}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Failed to delete tag {tag} for image {image}: {response.ReasonPhrase}");
            }
        }

        private static IEnumerable<Tag> ParseTags(string content)
        {
            using (JsonDocument document = JsonDocument.Parse(content))
            {
                JsonElement root = document.RootElement;
                JsonElement tagsElement = root.GetProperty("tags");
                var tags = new List<Tag>();

                foreach (JsonElement tagElement in tagsElement.EnumerateArray())
                {
                    tags.Add(new(tagElement.GetString(), tagElement.GetString())); // TODO: Get digest
                }

                return tags;
            }
        }
    }
}
