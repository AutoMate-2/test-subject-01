using to_integrations.HelperMethods;

namespace to_integrations.CRUD.Cities
{
    public class CitiesPresetup
    {
        public string ValidAgentId => AppConfig.GetValue("AgentId") ?? "username";
        public string ValidAgentPassword => AppConfig.GetValue("AgentPassword") ?? "password";
    }
}
