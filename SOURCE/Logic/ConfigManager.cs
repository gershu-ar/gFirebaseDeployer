using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using gFirebaseDeployer.Models;

namespace gFirebaseDeployer.Logic
{
    public static class ConfigManager
    {
        private static readonly string ConfigPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "gFirebaseDeployer", "config.json");

        public static AppConfig Config { get; private set; } = new();

        public static void Load()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    var json = File.ReadAllText(ConfigPath);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var loaded = JsonSerializer.Deserialize<AppConfig>(json, options);
                    if (loaded != null)
                        Config = loaded;
                }
                else
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath)!);
                    Save(); // create default config
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load config: {ex.Message}");
            }
        }

        public static void Save()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                var json = JsonSerializer.Serialize(Config, options);
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath)!);
                File.WriteAllText(ConfigPath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save config: {ex.Message}");
            }
        }
    }
}
