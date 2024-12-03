using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ContainerTagRemover.Interfaces;
using ContainerTagRemover.Services;
using Moq;
using Shouldly;
using Xunit;

namespace ContainerTagRemover.Tests.Services
{
    public class DockerhubClientTests
    {
        private readonly Mock<IAuthenticationClient> _mockAuthenticationClient;
        private readonly Mock<HttpClient> _mockHttpClient;
        private readonly DockerhubClient _dockerhubClient;

        public DockerhubClientTests()
        {
            _mockAuthenticationClient = new Mock<IAuthenticationClient>();
            _mockHttpClient = new Mock<HttpClient>();
            _dockerhubClient = new DockerhubClient(_mockAuthenticationClient.Object, _mockHttpClient.Object);
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
            var tags = new List<string> { "tag1", "tag2" };
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent("[\"tag1\", \"tag2\"]")
            };

            _mockHttpClient.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>())).ReturnsAsync(responseMessage);

            // Act
            var result = await _dockerhubClient.ListTagsAsync(repository);

            // Assert
            result.ShouldBe(tags);
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

            _mockHttpClient.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>())).ReturnsAsync(responseMessage);

            // Act
            await _dockerhubClient.DeleteTagAsync(repository, tag);

            // Assert
            _mockHttpClient.Verify(x => x.SendAsync(It.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Delete &&
                req.RequestUri == new Uri($"https://hub.docker.com/v2/repositories/{repository}/tags/{tag}")
            )), Times.Once);
        }
    }
}
