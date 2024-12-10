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
            services.AddHttpClient();
            services.AddSingleton<IContainerRegistryClient, DockerhubClient>();
            services.AddSingleton<IContainerRegistryClient, AzureContainerRegistryClient>(sp => new AzureContainerRegistryClient("https://test.azurecr.io"));
            services.AddSingleton<TagRemovalService>();
            services.AddSingleton((s) => new TagRemovalConfig()
            {
                Major = 2,
                Minor = 2
            });

            return services;
        }
    }
}
