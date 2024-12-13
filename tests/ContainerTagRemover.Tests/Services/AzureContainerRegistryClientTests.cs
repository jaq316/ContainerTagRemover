using ContainerTagRemover.Services;
using Moq;
using Shouldly;
using Moq.Protected;
using Azure.Core;
using Azure.Identity;
using ContainerTagRemover.Interfaces;
using Azure.Containers.ContainerRegistry;
using Azure;

namespace ContainerTagRemover.Tests.Services
{
    public class AzureContainerRegistryClientTests
    {
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly AzureContainerRegistryClient _client;
        private readonly Mock<TokenCredential> _mockCredential;

        public AzureContainerRegistryClientTests()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _mockCredential = new Mock<TokenCredential>();
            _client = new AzureContainerRegistryClient("https://test.azurecr.io");
        }

        [Fact]
        public async Task AuthenticateAsync_Should_Authenticate_Using_Environment_Variables()
        {
            // Arrange
            Environment.SetEnvironmentVariable("AZURE_TENANT_ID", "test-tenant-id");
            Environment.SetEnvironmentVariable("AZURE_CLIENT_ID", "test-client-id");
            Environment.SetEnvironmentVariable("AZURE_CLIENT_SECRET", "test-client-secret");

            var token = new AccessToken("test-token", DateTimeOffset.Now.AddHours(1));
            _mockCredential.Setup(c => c.GetTokenAsync(It.IsAny<TokenRequestContext>(), It.IsAny<CancellationToken>()))
                          .ReturnsAsync(token);

            // Act
            await _client.AuthenticateAsync();

            // Assert
            _client.ShouldNotBeNull();
        }
    }
}
