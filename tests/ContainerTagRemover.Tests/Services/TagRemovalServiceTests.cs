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
            var tags = new List<Tag> {
                new Tag("1.0.0", "digest1"),
                new Tag("1.0.1", "digest2"),
                new Tag("1.0.2", "digest3"),
                new Tag("1.0.3", "digest4"),
                new Tag("1.0.4", "digest5"),
                new Tag("1.0.5", "digest6"),
                new Tag("1.0.6", "digest7"),
                new Tag("1.0.7", "digest8"),
                new Tag("1.1.0", "digest9"),
                new Tag("1.1.1", "digest10"),
                new Tag("1.1.2", "digest11"),
                new Tag("1.1.3", "digest12"),
                new Tag("1.1.4", "digest13"),
                new Tag("1.1.5", "digest14"),
                new Tag("1.1.6", "digest15"),
                new Tag("1.1.7", "digest16"),
                new Tag("2.0.0", "digest17"),
                new Tag("2.0.1", "digest18"),
                new Tag("2.0.2", "digest19"),
                new Tag("2.0.3", "digest20"),
                new Tag("2.0.4", "digest21"),
                new Tag("2.0.5", "digest22"),
                new Tag("2.0.6", "digest23"),
                new Tag("2.0.7", "digest24"),
                new Tag("2.1.0", "digest25"),
                new Tag("2.1.1", "digest26"),
                new Tag("2.1.2", "digest27"),
                new Tag("2.1.3", "digest28"),
                new Tag("2.1.4", "digest29"),
                new Tag("2.1.5", "digest30"),
                new Tag("2.1.6", "digest31"),
                new Tag("2.1.7", "digest32"),
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

        [Fact]
        public void DetermineTagsToRemove_Should_Filter_Latest_Major_Versions()
        {
            // Arrange
            var tags = new List<string> {
                "1.0.0",
                "1.1.0",
                "2.0.0",
                "2.1.0",
                "3.0.0",
                "3.1.0",
                "4.0.0",
                "4.1.0",
            };

            // Act
            var result = _tagRemovalService.DetermineTagsToRemove(tags);

            // Assert
            result.ShouldNotContain("3.0.0");
            result.ShouldNotContain("3.1.0");
            result.ShouldNotContain("4.0.0");
            result.ShouldNotContain("4.1.0");
        }

        [Fact]
        public void DetermineTagsToRemove_Should_Filter_Latest_Minor_Versions_For_Each_Major_Version()
        {
            // Arrange
            var tags = new List<string> {
                "1.0.0",
                "1.1.0",
                "2.0.0",
                "2.1.0",
                "3.0.0",
                "3.1.0",
                "4.0.0",
                "4.1.0",
                "4.2.0",
                "4.3.0",
            };

            // Act
            var result = _tagRemovalService.DetermineTagsToRemove(tags);

            // Assert
            result.ShouldNotContain("4.2.0");
            result.ShouldNotContain("4.3.0");
        }

        [Fact]
        public void GetRemovedTags_Should_Return_Correct_List()
        {
            // Arrange
            var repository = "test-repo";
            var tags = new List<Tag> {
                new Tag("1.0.0", "digest1"),
                new Tag("1.0.1", "digest2"),
                new Tag("1.0.2", "digest3"),
                new Tag("1.0.3", "digest4"),
                new Tag("1.0.4", "digest5"),
                new Tag("1.0.5", "digest6"),
                new Tag("1.0.6", "digest7"),
                new Tag("1.0.7", "digest8"),
            };
            _mockRegistryClient.Setup(x => x.ListTagsAsync(repository)).ReturnsAsync(tags);

            // Act
            _tagRemovalService.RemoveOldTagsAsync(repository).Wait();
            var removedTags = _tagRemovalService.GetRemovedTags();

            // Assert
            removedTags.ShouldContain("1.0.0");
            removedTags.ShouldContain("1.0.1");
            removedTags.ShouldContain("1.0.2");
            removedTags.ShouldContain("1.0.3");
            removedTags.ShouldContain("1.0.4");
            removedTags.ShouldContain("1.0.5");
        }

        [Fact]
        public void GetKeptTags_Should_Return_Correct_List()
        {
            // Arrange
            var repository = "test-repo";
            var tags = new List<Tag> {
                new Tag("1.0.0", "digest1"),
                new Tag("1.0.1", "digest2"),
                new Tag("1.0.2", "digest3"),
                new Tag("1.0.3", "digest4"),
                new Tag("1.0.4", "digest5"),
                new Tag("1.0.5", "digest6"),
                new Tag("1.0.6", "digest7"),
                new Tag("1.0.7", "digest8"),
            };
            _mockRegistryClient.Setup(x => x.ListTagsAsync(repository)).ReturnsAsync(tags);

            // Act
            _tagRemovalService.RemoveOldTagsAsync(repository).Wait();
            var keptTags = _tagRemovalService.GetKeptTags();

            // Assert
            keptTags.ShouldContain("1.0.6");
            keptTags.ShouldContain("1.0.7");
        }
    }
}
