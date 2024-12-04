using ContainerTagRemover.Configuration;
using ContainerTagRemover.Interfaces;
using ContainerTagRemover.Services;
using Moq;
using Shouldly;

namespace ContainerTagRemover.Tests.Services
{
    public class TagRemovalServiceTests
    {
        private readonly Mock<IContainerRegistryClient> _mockRegistryClient;
        private readonly TagRemovalConfig _config;
        private readonly TagRemovalService _tagRemovalService;

        public TagRemovalServiceTests()
        {
            _mockRegistryClient = new Mock<IContainerRegistryClient>();
            _config = new TagRemovalConfig
            {
                Major = 2,
                Minor = 2
            };

            _tagRemovalService = new TagRemovalService(_mockRegistryClient.Object, _config);
        }

        [Fact]
        public async Task RemoveOldTagsAsync_Should_Remove_Old_Tags()
        {
            // Arrange
            var repository = "test-repo";
            var tags = new List<string> {
                "1.0.0",
                "1.0.1",
                "1.0.2",
                "1.0.3",
                "1.0.4",
                "1.0.5",
                "1.0.6",
                "1.0.7",
                "1.1.0",
                "1.1.1",
                "1.1.2",
                "1.1.3",
                "1.1.4",
                "1.1.5",
                "1.1.6",
                "1.1.7",
                "2.0.0",
                "2.0.1",
                "2.0.2",
                "2.0.3",
                "2.0.4",
                "2.0.5",
                "2.0.6",
                "2.0.7",
                "2.1.0",
                "2.1.1",
                "2.1.2",
                "2.1.3",
                "2.1.4",
                "2.1.5",
                "2.1.6",
                "2.1.7",
            };
            _mockRegistryClient.Setup(x => x.ListTagsAsync(repository)).ReturnsAsync(tags);

            // Act
            await _tagRemovalService.RemoveOldTagsAsync(repository);

            // Assert
            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "1.0.0"), Times.Once);
            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "1.0.1"), Times.Once);
            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "1.0.2"), Times.Once);
            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "1.0.3"), Times.Once);
            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "1.0.4"), Times.Once);
            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "1.0.5"), Times.Once);
            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "1.0.6"), Times.Never);
            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "1.0.7"), Times.Never);

            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "1.0.1"), Times.Once);
            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "1.1.0"), Times.Once);
            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "1.1.1"), Times.Once);
            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "1.1.2"), Times.Once);
            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "1.1.3"), Times.Once);
            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "1.1.4"), Times.Once);
            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "1.1.5"), Times.Once);
            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "1.1.6"), Times.Never);
            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "1.1.7"), Times.Never);

            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "2.0.0"), Times.Once);
            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "2.0.1"), Times.Once);
            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "2.0.2"), Times.Once);
            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "2.0.3"), Times.Once);
            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "2.0.4"), Times.Once);
            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "2.0.5"), Times.Once);
            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "2.0.6"), Times.Never);
            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "2.0.7"), Times.Never);

            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "2.1.0"), Times.Once);
            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "2.1.1"), Times.Once);
            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "2.1.2"), Times.Once);
            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "2.1.3"), Times.Once);
            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "2.1.4"), Times.Once);
            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "2.1.5"), Times.Once);
            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "2.1.6"), Times.Never);
            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "2.1.7"), Times.Never);
        }

        [Fact]
        public void DetermineTagsToRemove_Should_Return_Correct_Tags()
        {
            // Arrange
            var tags = new List<string> { 
                "1.0.0", 
                "1.0.1", 
                "1.0.2",
                "1.0.3",
                "1.0.4",
                "1.0.5",
                "1.0.6",
                "1.0.7",
                "1.1.0", 
                "1.1.1", 
                "1.1.2",
                "1.1.3",
                "1.1.4",
                "1.1.5",
                "1.1.6",
                "1.1.7",
                "2.0.0",
                "2.0.1",
                "2.0.2",
                "2.0.3",
                "2.0.4",
                "2.0.5",
                "2.0.6",
                "2.0.7",
                "2.1.0",
                "2.1.1",
                "2.1.2",
                "2.1.3",
                "2.1.4",
                "2.1.5",
                "2.1.6",
                "2.1.7",
            };

            // Act
            var result = _tagRemovalService.DetermineTagsToRemove(tags);

            // Assert
            result.ShouldContain("1.0.0");
            result.ShouldContain("1.0.1");
            result.ShouldContain("1.0.2");
            result.ShouldContain("1.0.3");
            result.ShouldContain("1.0.4");
            result.ShouldContain("1.0.5");
            result.ShouldContain("1.1.0");
            result.ShouldContain("1.1.1");
            result.ShouldContain("1.1.2");
            result.ShouldContain("1.1.3");
            result.ShouldContain("1.1.4");
            result.ShouldContain("1.1.5");
            result.ShouldContain("2.0.0");
            result.ShouldContain("2.0.1");
            result.ShouldContain("2.0.2");
            result.ShouldContain("2.0.3");
            result.ShouldContain("2.0.4");
            result.ShouldContain("2.0.5");
            result.ShouldContain("2.1.0");
            result.ShouldContain("2.1.1");
            result.ShouldContain("2.1.2");
            result.ShouldContain("2.1.3");
            result.ShouldContain("2.1.4");
            result.ShouldContain("2.1.5");
        }
    }
}
