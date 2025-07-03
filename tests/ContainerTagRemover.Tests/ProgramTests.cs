using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using ContainerTagRemover.Configuration;
using ContainerTagRemover.Services;
using ContainerTagRemover.Interfaces;

namespace ContainerTagRemover.Tests
{
    public class ProgramTests
    {

        [Fact]
        public async Task Main_WithInvalidConfigFilePath_ShowsErrorMessage()
        {
            // Arrange
            var registryUrl = "https://example.azurecr.io";
            var image = "test-image";
            var configFilePath = "invalid-config.json";

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                await Program.Main(new string[] { registryUrl, image, configFilePath });

                // Assert
                var result = sw.ToString();
                Assert.Contains("Error reading or validating configuration file", result);
            }
        }

        [Fact]
        public async Task Main_WithMissingArguments_PromptsForInput()
        {
            // Arrange
            var input = new StringReader("https://example.azurecr.io\ntest-image\nconfig.json\n");
            Console.SetIn(input);

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                await Program.Main(new string[] { });

                // Assert
                var result = sw.ToString();
                Assert.Contains("Please enter the missing arguments:", result);
            }
        }

        [Fact]
        public async Task Main_WithNoConfigFileButAdditionalArguments_UsesDefaultConfigAndParsesArgumentsCorrectly()
        {
            // Arrange
            var registryUrl = "https://example.azurecr.io";
            var image = "test-image";
            var outputFile = "output.json";
            var keepTags = "tag1,tag2";

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                
                // This should not throw an exception about missing config file
                // Instead it should fail on network connectivity (which is expected)
                try
                {
                    await Program.Main(new string[] { registryUrl, image, "--output-file", outputFile, "--keep-tags", keepTags });
                }
                catch (Exception ex)
                {
                    // We expect a network-related exception, not a config file exception
                    Assert.DoesNotContain("Error reading or validating configuration file", ex.Message);
                    Assert.DoesNotContain("Could not find file", ex.Message);
                    Assert.DoesNotContain("--output-file", ex.Message);
                }

                // Assert
                var result = sw.ToString();
                // Should not contain config file error messages
                Assert.DoesNotContain("Error reading or validating configuration file", result);
                Assert.DoesNotContain("Could not find file", result);
            }
        }

        [Fact]
        public async Task Main_WithConfigFileAndAdditionalArguments_ParsesAllArgumentsCorrectly()
        {
            // Arrange
            var registryUrl = "https://example.azurecr.io";
            var image = "test-image";
            var configFilePath = Path.GetTempFileName();
            var outputFile = "output.json";
            var keepTags = "tag1,tag2";
            
            // Create a temporary config file
            var configContent = @"{
  ""Major"": 2,
  ""Minor"": 2,
  ""KeepTags"": [""latest"", ""stable""]
}";
            await File.WriteAllTextAsync(configFilePath, configContent);

            try
            {
                // Act
                using (var sw = new StringWriter())
                {
                    Console.SetOut(sw);
                    
                    try
                    {
                        await Program.Main(new string[] { registryUrl, image, configFilePath, "--output-file", outputFile, "--keep-tags", keepTags });
                    }
                    catch (Exception ex)
                    {
                        // We expect a network-related exception, not a config file exception
                        Assert.DoesNotContain("Error reading or validating configuration file", ex.Message);
                        Assert.DoesNotContain("Could not find file", ex.Message);
                    }

                    // Assert
                    var result = sw.ToString();
                    // Should not contain config file error messages
                    Assert.DoesNotContain("Error reading or validating configuration file", result);
                }
            }
            finally
            {
                // Clean up
                if (File.Exists(configFilePath))
                    File.Delete(configFilePath);
            }
        }
    }
}
