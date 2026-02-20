using System;
using to_integrations.HelperMethods;

namespace to_integrations.CRUD.Districts
{
    public static class DistrictsPresetup
    {
        public static string ValidAgentId => AppConfig.GetValue("AgentId") ?? "username";
        public static string ValidAgentPassword => AppConfig.GetValue("AgentPassword") ?? "password";
    }
}
