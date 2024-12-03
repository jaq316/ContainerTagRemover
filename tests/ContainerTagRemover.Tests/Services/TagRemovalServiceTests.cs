using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContainerTagRemover.Configuration;
using ContainerTagRemover.Interfaces;
using ContainerTagRemover.Services;
using Moq;
using Shouldly;
using Xunit;

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
            _config = new TagRemovalConfig { Major = 1, Minor = 1, Patch = 1 };
            _tagRemovalService = new TagRemovalService(_mockRegistryClient.Object, _config);
        }

        [Fact]
        public async Task RemoveOldTagsAsync_Should_Remove_Old_Tags()
        {
            // Arrange
            var repository = "test-repo";
            var tags = new List<string> { "1.0.0", "1.0.1", "1.1.0", "2.0.0" };
            _mockRegistryClient.Setup(x => x.ListTagsAsync(repository)).ReturnsAsync(tags);

            // Act
            await _tagRemovalService.RemoveOldTagsAsync(repository);

            // Assert
            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "1.0.0"), Times.Once);
            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "1.0.1"), Times.Once);
            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "1.1.0"), Times.Once);
            _mockRegistryClient.Verify(x => x.DeleteTagAsync(repository, "2.0.0"), Times.Never);
        }

        [Fact]
        public void DetermineTagsToRemove_Should_Return_Correct_Tags()
        {
            // Arrange
            var tags = new List<string> { "1.0.0", "1.0.1", "1.1.0", "2.0.0" };

            // Act
            var result = _tagRemovalService.DetermineTagsToRemove(tags);

            // Assert
            result.ShouldBe(new List<string> { "1.0.0", "1.0.1", "1.1.0" });
        }
    }
}
