using System;
using System.Collections.Generic;
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
            string configContent = "{\"Major\": 5, \"Minor\": 10}";
            string tempFilePath = Path.GetTempFileName();
            File.WriteAllText(tempFilePath, configContent);

            // Act
            TagRemovalConfig config = TagRemovalConfig.Load(tempFilePath);

            // Assert
            config.Major.ShouldBe(5);
            config.Minor.ShouldBe(10);

            // Cleanup
            File.Delete(tempFilePath);
        }

        [Fact]
        public void LoadFromFile_ShouldReturnDefaultValues_WhenFilePathIsNotSpecified()
        {
            // Act
            TagRemovalConfig config = TagRemovalConfig.Load((string?)null);

            // Assert
            config.Major.ShouldBe(2);
            config.Minor.ShouldBe(2);
        }

        [Fact]
        public void LoadFromFile_ShouldThrowInvalidOperationException_WhenFileContentIsInvalid()
        {
            // Arrange
            string invalidConfigContent = "invalid json";
            string tempFilePath = Path.GetTempFileName();
            File.WriteAllText(tempFilePath, invalidConfigContent);

            // Act & Assert
            Should.Throw<InvalidOperationException>(() => TagRemovalConfig.Load(tempFilePath));

            // Cleanup
            File.Delete(tempFilePath);
        }

        [Fact]
        public void Validate_ShouldThrowInvalidOperationException_WhenConfigValuesAreNegative()
        {
            // Arrange
            var config = new TagRemovalConfig { Major = -1, Minor = 10 };

            // Act & Assert
            Should.Throw<InvalidOperationException>(() => config.Validate());
        }

        [Fact]
        public void Validate_ShouldNotThrowException_WhenConfigValuesAreNonNegative()
        {
            // Arrange
            var config = new TagRemovalConfig { Major = 5, Minor = 10 };

            // Act & Assert
            Should.NotThrow(() => config.Validate());
        }

        [Fact]
        public void LoadFromFile_ShouldLoadKeepTagsSuccessfully()
        {
            // Arrange
            string configContent = "{\"Major\": 5, \"Minor\": 10, \"KeepTags\": [\"latest\", \"stable\", \"1.0.0\"]}";
            string tempFilePath = Path.GetTempFileName();
            File.WriteAllText(tempFilePath, configContent);

            // Act
            TagRemovalConfig config = TagRemovalConfig.Load(tempFilePath);

            // Assert
            config.Major.ShouldBe(5);
            config.Minor.ShouldBe(10);
            config.KeepTags.ShouldNotBeNull();
            config.KeepTags.Count.ShouldBe(3);
            config.KeepTags.ShouldContain("latest");
            config.KeepTags.ShouldContain("stable");
            config.KeepTags.ShouldContain("1.0.0");

            // Cleanup
            File.Delete(tempFilePath);
        }

        [Fact]
        public void LoadFromFile_ShouldInitializeEmptyKeepTags_WhenNotSpecified()
        {
            // Arrange
            string configContent = "{\"Major\": 5, \"Minor\": 10}";
            string tempFilePath = Path.GetTempFileName();
            File.WriteAllText(tempFilePath, configContent);

            // Act
            TagRemovalConfig config = TagRemovalConfig.Load(tempFilePath);

            // Assert
            config.KeepTags.ShouldNotBeNull();
            config.KeepTags.Count.ShouldBe(0);

            // Cleanup
            File.Delete(tempFilePath);
        }

        [Fact]
        public void KeepTags_ShouldBeInitializedByDefault()
        {
            // Act
            var config = new TagRemovalConfig();

            // Assert
            config.KeepTags.ShouldNotBeNull();
            config.KeepTags.Count.ShouldBe(0);
        }
    }
}
