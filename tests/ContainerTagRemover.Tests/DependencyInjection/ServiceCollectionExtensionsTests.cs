using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;
using Xunit;
using ContainerTagRemover.DependencyInjection;
using ContainerTagRemover.Interfaces;
using ContainerTagRemover.Services;
using Azure.Core;
using Azure.Identity;

namespace ContainerTagRemover.Tests.DependencyInjection
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddContainerTagRemoverServices_RegistersServicesCorrectly()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton<TokenCredential, DefaultAzureCredential>();

            // Act
            services.AddContainerTagRemoverServices();
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            var dockerhubClient = serviceProvider.GetService<IContainerRegistryClient>();
            var azureContainerRegistryClient = serviceProvider.GetService<IContainerRegistryClient>();
            var tagRemovalService = serviceProvider.GetService<TagRemovalService>();

            dockerhubClient.ShouldNotBeNull();
            azureContainerRegistryClient.ShouldNotBeNull();
            tagRemovalService.ShouldNotBeNull();
        }
    }
}
