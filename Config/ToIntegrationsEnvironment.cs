using System;

namespace to_integrations.Config
{
    public class ToIntegrationsEnvironment
    {
        public static string BaseUrl { get; private set; } = null!;
        public static string AgentId { get; private set; } = null!;
        public static string AgentPassword { get; private set; } = null!;


        public static void Initialize()
        {
            var cfg = AppConfig.Current?.ToIntegrationsConfig
                ?? throw new Exception("ToIntegrationsConfig missing");

            BaseUrl = cfg.BaseUrl!;
            AgentId = cfg.AgentId!;
            AgentPassword = cfg.AgentPassword!;
        }
    }
}
