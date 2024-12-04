using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ContainerTagRemover.Interfaces;
using ContainerTagRemover.Services;
using Moq;
using Shouldly;
using Xunit;
using System.Threading;
using Moq.Protected;

namespace ContainerTagRemover.Tests.Services
{
    public class AzureContainerRegistryClientTests
    {
        private readonly Mock<IAuthenticationClient> _mockAuthenticationClient;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly AzureContainerRegistryClient _client;

        public AzureContainerRegistryClientTests()
        {
            _mockAuthenticationClient = new Mock<IAuthenticationClient>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _client = new AzureContainerRegistryClient(_mockAuthenticationClient.Object, _httpClient);
        }

        [Fact]
        public async Task AuthenticateAsync_Should_Call_AuthenticateAsync_On_AuthenticationClient()
        {
            // Arrange
            _mockAuthenticationClient.Setup(x => x.AuthenticateAsync()).Returns(Task.CompletedTask);

            // Act
            await _client.AuthenticateAsync();

            // Assert
            _mockAuthenticationClient.Verify(x => x.AuthenticateAsync(), Times.Once);
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
            tags.ShouldBe(new List<string> { "v1.0.0", "v1.0.1" });
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
            await _client.DeleteTagAsync(repository, tag);

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
    }
}
