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
                        req.Content != null &&
                        VerifyRequestContent(req.Content)
                    ),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(responseMessage);

            // Act
            await _dockerhubClient.AuthenticateAsync();

            // Assert
            _dockerhubClient.ShouldNotBeNull();
        }

        private bool VerifyRequestContent(HttpContent content)
        {
            var contentString = content.ReadAsStringAsync().Result;
            contentString.ShouldContain("test-username");
            contentString.ShouldContain("test-password");
            return true;
        }
    }
}
