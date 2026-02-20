using System;
using to_integrations.HelperMethods;

namespace to_integrations.CRUD.Cities
{
    public static class CitiesPresetup
    {
        public static string ValidAgentId => AppConfig.GetValue("AgentId") ?? "username";
        public static string ValidAgentPassword => AppConfig.GetValue("AgentPassword") ?? "password";
    }
}
