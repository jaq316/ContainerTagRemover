using System;
using System.IO;
using ContainerTagRemover.Configuration;
using Shouldly;
using Xunit;

namespace ContainerTagRemover.Tests.Configuration
{
    public class TagRemovalConfigTests
    {
        [Fact]
        public void LoadFromFile_ShouldLoadConfigSuccessfully()
        {
            // Arrange
            string configContent = "{\"Major\": 5, \"Minor\": 10, \"Patch\": 20}";
            string tempFilePath = Path.GetTempFileName();
            File.WriteAllText(tempFilePath, configContent);

            // Act
            TagRemovalConfig config = TagRemovalConfig.LoadFromFile(tempFilePath);

            // Assert
            config.Major.ShouldBe(5);
            config.Minor.ShouldBe(10);
            config.Patch.ShouldBe(20);

            // Cleanup
            File.Delete(tempFilePath);
        }

        [Fact]
        public void LoadFromFile_ShouldThrowFileNotFoundException_WhenFileDoesNotExist()
        {
            // Arrange
            string nonExistentFilePath = "nonexistent.json";

            // Act & Assert
            Should.Throw<FileNotFoundException>(() => TagRemovalConfig.LoadFromFile(nonExistentFilePath));
        }

        [Fact]
        public void LoadFromFile_ShouldThrowInvalidOperationException_WhenFileContentIsInvalid()
        {
            // Arrange
            string invalidConfigContent = "invalid json";
            string tempFilePath = Path.GetTempFileName();
            File.WriteAllText(tempFilePath, invalidConfigContent);

            // Act & Assert
            Should.Throw<InvalidOperationException>(() => TagRemovalConfig.LoadFromFile(tempFilePath));

            // Cleanup
            File.Delete(tempFilePath);
        }

        [Fact]
        public void Validate_ShouldThrowInvalidOperationException_WhenConfigValuesAreNegative()
        {
            // Arrange
            var config = new TagRemovalConfig { Major = -1, Minor = 10, Patch = 20 };

            // Act & Assert
            Should.Throw<InvalidOperationException>(() => config.Validate());
        }

        [Fact]
        public void Validate_ShouldNotThrowException_WhenConfigValuesAreNonNegative()
        {
            // Arrange
            var config = new TagRemovalConfig { Major = 5, Minor = 10, Patch = 20 };

            // Act & Assert
            Should.NotThrow(() => config.Validate());
        }
    }
}
