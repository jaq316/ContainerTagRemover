using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using ContainerTagRemover.Interfaces;

namespace ContainerTagRemover.Services
{
    public class AzureContainerRegistryClient : IContainerRegistryClient
    {
        private readonly HttpClient _httpClient;
        private string _accessToken;

        public AzureContainerRegistryClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task AuthenticateAsync()
        {
            var tenantId = Environment.GetEnvironmentVariable("AZURE_TENANT_ID");
            var clientId = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID");
            var clientSecret = Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET");

            if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(clientId))
            {
                throw new InvalidOperationException("Environment variables AZURE_TENANT_ID and AZURE_CLIENT_ID must be set.");
            }

            var credential = string.IsNullOrEmpty(clientSecret)
                ? new DefaultAzureCredential()
                : new ClientSecretCredential(tenantId, clientId, clientSecret);

            var tokenRequestContext = new TokenRequestContext(new[] { "https://management.azure.com/.default" });
            var token = await credential.GetTokenAsync(tokenRequestContext);
            _accessToken = token.Token;
        }

        public async Task<IEnumerable<string>> ListTagsAsync(string repository)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://{repository}.azurecr.io/v2/_catalog");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);
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
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);
            var response = await _httpClient.SendAsync(request);

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
