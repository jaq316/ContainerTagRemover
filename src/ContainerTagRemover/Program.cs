using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ContainerTagRemover.Configuration;
using ContainerTagRemover.Services;
using ContainerTagRemover.Interfaces;

namespace ContainerTagRemover
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Usage: ContainerTagRemover <registry> <repository> <config-file>");
                return;
            }

            string registry = args[0];
            string repository = args[1];
            string configFilePath = args[2];

            if (!File.Exists(configFilePath))
            {
                Console.WriteLine($"Configuration file not found: {configFilePath}");
                return;
            }

            TagRemovalConfig config;
            try
            {
                string configContent = File.ReadAllText(configFilePath);
                config = JsonConvert.DeserializeObject<TagRemovalConfig>(configContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading configuration file: {ex.Message}");
                return;
            }

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection, registry);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var tagRemovalService = serviceProvider.GetService<TagRemovalService>();
            tagRemovalService.RemoveOldTags(repository, config);
        }

        private static void ConfigureServices(IServiceCollection services, string registry)
        {
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
