using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows.Forms;

namespace gFirebaseDeployer.Logic
{
    public static class LanguageManager
    {
        private static Dictionary<string, string> _strings = new();

        public static void Load(string languageCode)
        {
            string resourceName = languageCode switch
            {
                "es" => "gFirebaseDeployer.Resources.Languages.es.json",
                "en" => "gFirebaseDeployer.Resources.Languages.en.json",
                _ => throw new Exception($"Unsupported language code: {languageCode}")
            };

            var assembly = Assembly.GetExecutingAssembly();
            using Stream? stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                MessageBox.Show($"Embedded language file not found: {resourceName}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _strings = new();
                return;
            }

            using var reader = new StreamReader(stream);
            string json = reader.ReadToEnd();
            _strings = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
        }

        public static string Get(string key)
        {
            if (_strings.TryGetValue(key, out var value))
                return value;
            return $"[{key}]"; // Show missing keys clearly
        }
    }
}
