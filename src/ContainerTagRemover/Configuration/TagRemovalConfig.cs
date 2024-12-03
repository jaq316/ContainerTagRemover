using System;
using System.IO;
using Newtonsoft.Json;

namespace ContainerTagRemover.Configuration
{
    public class TagRemovalConfig
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Patch { get; set; }

        public static TagRemovalConfig LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Configuration file not found: {filePath}");
            }

            try
            {
                string configContent = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<TagRemovalConfig>(configContent);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error reading configuration file: {ex.Message}");
            }
        }

        public void Validate()
        {
            if (Major < 0 || Minor < 0 || Patch < 0)
            {
                throw new InvalidOperationException("Configuration values for Major, Minor, and Patch must be non-negative.");
            }
        }
    }
}
