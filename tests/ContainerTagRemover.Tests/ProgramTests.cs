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
    }
}
