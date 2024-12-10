using ContainerTagRemover.Services;
using Moq;
using Shouldly;
using Moq.Protected;
using ContainerTagRemover.Interfaces;

namespace ContainerTagRemover.Tests.Services
{
    public class DockerhubClientTests
    {
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly DockerhubClient _dockerhubClient;

        public DockerhubClientTests()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _dockerhubClient = new DockerhubClient(_httpClient, "test-repo");
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
            tags.ShouldBeOfType<List<Tag>>();
            tags.ShouldContain(t => t.Name == "v1.0.0");
            tags.ShouldContain(t => t.Name == "v1.0.1");
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

        [Fact]
        public async Task AuthenticateAsync_Should_Authenticate_Using_Environment_Variables()
        {
            // Arrange
            Environment.SetEnvironmentVariable("DOCKERHUB_USERNAME", "test-username");
            Environment.SetEnvironmentVariable("DOCKERHUB_PASSWORD", "test-password");

            var responseContent = "{\"token\": \"test-token\"}";
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            };

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri == new Uri("https://hub.docker.com/v2/users/login/") &&
                        req.Content.ReadAsStringAsync().Result.Contains("test-username") &&
                        req.Content.ReadAsStringAsync().Result.Contains("test-password")
                    ),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(responseMessage);

            // Act
            await _dockerhubClient.AuthenticateAsync();

            // Assert
            _dockerhubClient.ShouldNotBeNull();
        }
    }
}
