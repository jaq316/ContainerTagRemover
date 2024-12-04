using Microsoft.Extensions.DependencyInjection;
using ContainerTagRemover.Interfaces;
using ContainerTagRemover.Services;
using ContainerTagRemover.Configuration;

namespace ContainerTagRemover.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddContainerTagRemoverServices(this IServiceCollection services)
        {
            services.AddSingleton<IAuthenticationClient, DockerhubClient>();
            services.AddSingleton<IContainerRegistryClient, DockerhubClient>();
            services.AddSingleton<IContainerRegistryClient, AzureContainerRegistryClient>();
            services.AddSingleton<TagRemovalConfig>();

            return services;
        }
    }
}
