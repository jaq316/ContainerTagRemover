using System;
using System.IO;
using System.Text.Json;

namespace ContainerTagRemover.Configuration
{
    public class TagRemovalConfig
    {
        public int Major { get; set; }
        public int Minor { get; set; }

        public static TagRemovalConfig Load(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return new TagRemovalConfig { Major = 2, Minor = 2 };
            }

            try
            {
                FileStream configStream = File.Open(filePath, FileMode.Open);
                return Load(configStream);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error reading configuration file: {ex.Message}");
            }
        }

        public static TagRemovalConfig Load(Stream stream)
        {
            try
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string configContent = reader.ReadToEnd();
                    return JsonSerializer.Deserialize<TagRemovalConfig>(configContent);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error reading configuration file: {ex.Message}");
            }
        }

        public void Validate()
        {
            if (Major < 0 || Minor < 0)
            {
                throw new InvalidOperationException("Configuration values for Major and Minor must be non-negative.");
            }
        }
    }
}
