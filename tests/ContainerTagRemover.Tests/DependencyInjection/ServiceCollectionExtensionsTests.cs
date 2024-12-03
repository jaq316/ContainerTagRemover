using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;
using Xunit;
using ContainerTagRemover.DependencyInjection;
using ContainerTagRemover.Interfaces;
using ContainerTagRemover.Services;

namespace ContainerTagRemover.Tests.DependencyInjection
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddContainerTagRemoverServices_RegistersServicesCorrectly()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddContainerTagRemoverServices();
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            var authenticationClient = serviceProvider.GetService<IAuthenticationClient>();
            var dockerhubClient = serviceProvider.GetService<IContainerRegistryClient>();
            var azureContainerRegistryClient = serviceProvider.GetService<IContainerRegistryClient>();
            var tagRemovalService = serviceProvider.GetService<TagRemovalService>();

            authenticationClient.ShouldNotBeNull();
            dockerhubClient.ShouldNotBeNull();
            azureContainerRegistryClient.ShouldNotBeNull();
            tagRemovalService.ShouldNotBeNull();
        }
    }
}
