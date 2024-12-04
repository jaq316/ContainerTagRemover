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
    public class DockerhubClientTests
    {
        private readonly Mock<IAuthenticationClient> _mockAuthenticationClient;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly DockerhubClient _dockerhubClient;

        public DockerhubClientTests()
        {
            _mockAuthenticationClient = new Mock<IAuthenticationClient>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _dockerhubClient = new DockerhubClient(_mockAuthenticationClient.Object, _httpClient);
        }

        [Fact]
        public async Task AuthenticateAsync_Should_Call_AuthenticateAsync_On_AuthenticationClient()
        {
            // Arrange
            _mockAuthenticationClient.Setup(x => x.AuthenticateAsync()).Returns(Task.CompletedTask);

            // Act
            await _dockerhubClient.AuthenticateAsync();

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
            var tags = await _dockerhubClient.ListTagsAsync(repository);

            // Assert
            tags.ShouldBe(new List<string> { "v1.0.0", "v1.0.1" });
        }

        [Fact]
        public async Task DeleteTagAsync_Should_Call_Delete_On_HttpClient()
        {
            // Arrange
            var repository = "test-repo";
            var tag = "test-tag";
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
            await _dockerhubClient.DeleteTagAsync(repository, tag);

            // Assert
            _mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Delete &&
                    req.RequestUri == new Uri($"https://hub.docker.com/v2/repositories/{repository}/tags/{tag}")
                ),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}
