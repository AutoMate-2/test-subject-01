using System;
using System.IO;
using System.Text.Json;

namespace to_integrations.HelperMethods
{
    public static class AppConfig
    {
        private static JsonDocument _configDocument;
        private static bool _isLoaded = false;

        public static void Load(string configFileName)
        {
            if (_isLoaded) return;

            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFileName);
            if (File.Exists(configPath))
            {
                var jsonContent = File.ReadAllText(configPath);
                _configDocument = JsonDocument.Parse(jsonContent);
                _isLoaded = true;
            }
        }

        public static string GetValue(string key)
        {
            if (_configDocument == null) return null;

            if (_configDocument.RootElement.TryGetProperty(key, out var value))
            {
                return value.GetString();
            }

            return null;
        }
    }
}
