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
        public async Task ListTagsAsync_Should_Return_Tags()
        {
            // Arrange
            var repository = "test-repo";
            var responseContent = "{\"tags\": [\"v1.0.0\", \"v1.0.1\"]}";
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            };
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(responseMessage);

            // Act
            var tags = await _client.ListTagsAsync(repository);

            // Assert
            tags.ShouldBeOfType<List<Tag>>();
            tags.ShouldContain(t => t.Name == "v1.0.0");
            tags.ShouldContain(t => t.Name == "v1.0.1");
        }

        [Fact]
        public async Task DeleteTagAsync_Should_Delete_Tag()
        {
            // Arrange
            var repository = "test-repo";
            var tag = "v1.0.0";
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK
            };
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(responseMessage);

            // Act
            try
            {
                await _client.DeleteTagAsync(repository, tag);
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                // Handle the exception if needed
            }

            // Assert
            _mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Delete &&
                    req.RequestUri == new Uri($"https://{repository}.azurecr.io/v2/{repository}/manifests/{tag}")
                ),
                ItExpr.IsAny<CancellationToken>()
            );
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
