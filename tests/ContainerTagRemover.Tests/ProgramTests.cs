using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Moq;
using ContainerTagRemover;
using ContainerTagRemover.Configuration;
using ContainerTagRemover.Interfaces;
using ContainerTagRemover.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace ContainerTagRemover.Tests
{
    public class ProgramTests
    {
        [Fact]
        public async Task Main_Should_Write_Output_To_File_When_OutputFile_Parameter_Is_Provided()
        {
            // Arrange
            var registryUrl = "https://test.azurecr.io";
            var image = "test-image";
            var configFilePath = "test-config.json";
            var outputFilePath = "output.json";

            var args = new[] { registryUrl, image, configFilePath, "--output-file", outputFilePath };

            var mockRegistryClient = new Mock<IContainerRegistryClient>();
            mockRegistryClient.Setup(x => x.AuthenticateAsync(It.IsAny<System.Threading.CancellationToken>())).Returns(Task.CompletedTask);
            mockRegistryClient.Setup(x => x.ListTagsAsync(It.IsAny<string>())).ReturnsAsync(new[] { new Tag("1.0.0", "digest1"), new Tag("1.0.1", "digest2") });
            mockRegistryClient.Setup(x => x.DeleteTagAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(mockRegistryClient.Object);
            serviceCollection.AddSingleton(new TagRemovalConfig { Major = 2, Minor = 2 });
            serviceCollection.AddSingleton<TagRemovalService>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
            mockServiceScopeFactory.Setup(x => x.CreateScope()).Returns(serviceScopeFactory.CreateScope());

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(mockServiceScopeFactory.Object);

            // Act
            await Program.Main(args);

            // Assert
            Assert.True(File.Exists(outputFilePath));

            var outputContent = await File.ReadAllTextAsync(outputFilePath);
            var output = JsonSerializer.Deserialize<dynamic>(outputContent);

            Assert.NotNull(output);
            Assert.Equal(2, ((JsonElement)output).GetProperty("RemovedTags").GetArrayLength());
            Assert.Equal(0, ((JsonElement)output).GetProperty("KeptTags").GetArrayLength());

            // Cleanup
            File.Delete(outputFilePath);
        }

        [Fact]
        public async Task Main_Should_Not_Write_Output_To_File_When_OutputFile_Parameter_Is_Not_Provided()
        {
            // Arrange
            var registryUrl = "https://test.azurecr.io";
            var image = "test-image";
            var configFilePath = "test-config.json";

            var args = new[] { registryUrl, image, configFilePath };

            var mockRegistryClient = new Mock<IContainerRegistryClient>();
            mockRegistryClient.Setup(x => x.AuthenticateAsync(It.IsAny<System.Threading.CancellationToken>())).Returns(Task.CompletedTask);
            mockRegistryClient.Setup(x => x.ListTagsAsync(It.IsAny<string>())).ReturnsAsync(new[] { new Tag("1.0.0", "digest1"), new Tag("1.0.1", "digest2") });
            mockRegistryClient.Setup(x => x.DeleteTagAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(mockRegistryClient.Object);
            serviceCollection.AddSingleton(new TagRemovalConfig { Major = 2, Minor = 2 });
            serviceCollection.AddSingleton<TagRemovalService>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
            mockServiceScopeFactory.Setup(x => x.CreateScope()).Returns(serviceScopeFactory.CreateScope());

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(mockServiceScopeFactory.Object);

            // Act
            await Program.Main(args);

            // Assert
            Assert.False(File.Exists("output.json"));
        }
    }
}
