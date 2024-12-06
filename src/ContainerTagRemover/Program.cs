using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ContainerTagRemover.Configuration;
using ContainerTagRemover.Services;
using ContainerTagRemover.Interfaces;

namespace ContainerTagRemover
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            string registry, repository, configFilePath;

            if (args.Length != 3)
            {
                Console.WriteLine("Usage: ContainerTagRemover <registry> <repository> <config-file>");
                Console.WriteLine("Please enter the missing arguments:");

                Console.Write("Registry: ");
                registry = Console.ReadLine();

                Console.Write("Repository: ");
                repository = Console.ReadLine();

                Console.Write("Config File Path: ");
                configFilePath = Console.ReadLine();
            }
            else
            {
                registry = args[0];
                repository = args[1];
                configFilePath = args[2];
            }

            TagRemovalConfig config;
            try
            {
                if (string.IsNullOrEmpty(configFilePath))
                {
                    config = new TagRemovalConfig { Major = 2, Minor = 2 };
                }
                else
                {
                    config = TagRemovalConfig.Load(configFilePath);
                    config.Validate();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading or validating configuration file: {ex.Message}");
                return;
            }

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection, registry, config);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var registryClient = serviceProvider.GetService<IContainerRegistryClient>();
            await registryClient.AuthenticateAsync();

            var tagRemovalService = serviceProvider.GetService<TagRemovalService>();
            await tagRemovalService.RemoveOldTagsAsync(repository);
        }

        private static void ConfigureServices(IServiceCollection services, string registry, TagRemovalConfig config)
        {
            services.AddSingleton(config);
            services.AddSingleton<TagRemovalService>();

            switch (registry.ToLower())
            {
                case "dockerhub":
                    services.AddSingleton<IContainerRegistryClient, DockerhubClient>();
                    break;
                case "azure":
                    services.AddSingleton<IContainerRegistryClient, AzureContainerRegistryClient>();
                    break;
                default:
                    throw new ArgumentException($"Unsupported registry: {registry}");
            }
        }
    }
}
