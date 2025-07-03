using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ContainerTagRemover.Configuration;
using ContainerTagRemover.Services;
using ContainerTagRemover.Interfaces;
using System.Net.Http;
using System.Text.Json;

namespace ContainerTagRemover
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            string registryUrl, image, configFilePath, outputFilePath = null;
            var keepTags = new List<string>();

            if (args.Length < 2)
            {
                Console.WriteLine("Usage: ContainerTagRemover <registry-url> <image> <config-file> [--output-file <output-file>] [--keep-tags <tag1,tag2,...>]");
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
                configFilePath = args.Length > 2 ? args[2] : null;

                for (int i = 3; i < args.Length; i++)
                {
                    if (args[i] == "--output-file" && i + 1 < args.Length)
                    {
                        outputFilePath = args[i + 1];
                        i++; // Skip the next argument as it's the value
                    }
                    else if (args[i] == "--keep-tags" && i + 1 < args.Length)
                    {
                        var tagList = args[i + 1];
                        keepTags.AddRange(tagList.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(tag => tag.Trim()));
                        i++; // Skip the next argument as it's the value
                    }
                }
            }

            image = image.ToLower();

            TagRemovalConfig config;
            try
            {
                config = TagRemovalConfig.Load(configFilePath);
                
                // Merge command line keep tags with configuration keep tags
                if (keepTags.Any())
                {
                    config.KeepTags.AddRange(keepTags);
                }
                
                config.Validate();
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

            var removedTags = tagRemovalService.GetRemovedTags();
            Console.WriteLine("Removed Tags:");
            foreach (var tag in removedTags)
            {
                Console.WriteLine(tag);
            }

            var keptTags = tagRemovalService.GetKeptTags();
            Console.WriteLine("Kept Tags:");
            foreach (var tag in keptTags)
            {
                Console.WriteLine(tag);
            }

            Console.WriteLine($"Summary: Removed {removedTags.Count} tags, Kept {keptTags.Count} tags.");

            if (!string.IsNullOrEmpty(outputFilePath))
            {
                var output = new
                {
                    RemovedTags = removedTags,
                    KeptTags = keptTags
                };

                try
                {
                    string jsonOutput = JsonSerializer.Serialize(output, new JsonSerializerOptions { WriteIndented = true });
                    await File.WriteAllTextAsync(outputFilePath, jsonOutput);
                    Console.WriteLine($"Output written to {outputFilePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error writing to output file: {ex.Message}");
                }
            }
        }

        private static void ConfigureServices(IServiceCollection services, string registryUrl, TagRemovalConfig config)
        {
            services.AddSingleton(config);
            services.AddSingleton<TagRemovalService>();

            if (registryUrl.Contains("azurecr.io"))
            {
                services.AddSingleton<IContainerRegistryClient>(sp => new AzureContainerRegistryClient(registryUrl));
            }
            else if (registryUrl.Contains("dockerhub"))
            {
                services.AddSingleton<IContainerRegistryClient>(sp => new DockerhubClient(sp.GetRequiredService<HttpClient>(), registryUrl));
            }
            else
            {
                throw new ArgumentException($"Unsupported registry URL: {registryUrl}");
            }
        }
    }
}
