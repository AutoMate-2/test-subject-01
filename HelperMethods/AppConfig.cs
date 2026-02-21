using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace to_integrations.HelperMethods
{
    public static class AppConfig
    {
        private static Dictionary<string, string> _config = new Dictionary<string, string>();

        public static void Load(string configFileName)
        {
            if (!File.Exists(configFileName))
                return;

            var json = File.ReadAllText(configFileName);
            using var doc = JsonDocument.Parse(json);
            foreach (var property in doc.RootElement.EnumerateObject())
            {
                _config[property.Name] = property.Value.ToString();
            }
        }

        public static string GetValue(string key)
        {
            return _config.ContainsKey(key) ? _config[key] : null;
        }
    }
}
