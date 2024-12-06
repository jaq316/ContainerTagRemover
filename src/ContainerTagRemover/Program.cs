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
            string registryUrl, image, configFilePath;

            if (args.Length != 3)
            {
                Console.WriteLine("Usage: ContainerTagRemover <registry-url> <image> <config-file>");
                Console.WriteLine("Please enter the missing arguments:");

                Console.Write("Registry URL: ");
                registryUrl = Console.ReadLine();

                Console.Write("Image: ");
                image = Console.ReadLine();

                Console.Write("Config File Path: ");
                configFilePath = Console.ReadLine();
            }
            else
            {
                registryUrl = args[0];
                image = args[1];
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
            ConfigureServices(serviceCollection, registryUrl, config);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var registryClient = serviceProvider.GetService<IContainerRegistryClient>();
            await registryClient.AuthenticateAsync();

            var tagRemovalService = serviceProvider.GetService<TagRemovalService>();
            await tagRemovalService.RemoveOldTagsAsync(image);
        }

        private static void ConfigureServices(IServiceCollection services, string registryUrl, TagRemovalConfig config)
        {
            services.AddSingleton(config);
            services.AddSingleton<TagRemovalService>();

            if (registryUrl.Contains("azurecr.io"))
            {
                services.AddSingleton<IContainerRegistryClient, AzureContainerRegistryClient>();
            }
            else if (registryUrl.Contains("dockerhub"))
            {
                services.AddSingleton<IContainerRegistryClient, DockerhubClient>();
            }
            else
            {
                throw new ArgumentException($"Unsupported registry URL: {registryUrl}");
            }
        }
    }
}
