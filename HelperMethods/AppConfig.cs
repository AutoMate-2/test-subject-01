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
            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFileName);
            
            if (!File.Exists(configPath))
            {
                configPath = Path.Combine(Directory.GetCurrentDirectory(), configFileName);
            }

            if (File.Exists(configPath))
            {
                var jsonContent = File.ReadAllText(configPath);
                _configDocument = JsonDocument.Parse(jsonContent);
                _isLoaded = true;
            }
            else
            {
                _isLoaded = false;
            }
        }

        public static string GetValue(string key)
        {
            if (!_isLoaded || _configDocument == null)
            {
                return null;
            }

            try
            {
                if (_configDocument.RootElement.TryGetProperty(key, out var element))
                {
                    return element.GetString();
                }
            }
            catch
            {
                return null;
            }

            return null;
        }
    }
}
