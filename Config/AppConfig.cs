using Newtonsoft.Json;
using System.IO;

namespace to_integrations.Config
{
    public class AppConfig
    {
        public static AppConfig? Current { get; private set; }

        public ToIntegrationsConfiguration? ToIntegrationsConfig { get; set; }

        public string[] CountryCodes { get; set; } = [];

        public static void Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Config file not found at {filePath}");

            var json = File.ReadAllText(filePath);
            Current = JsonConvert.DeserializeObject<AppConfig>(json);
        }

        public class ToIntegrationsConfiguration
        {
            public string? BaseUrl { get; set; }
            public string? AgentId { get; set; }
            public string? AgentPassword { get; set; }
        }
    }
}
