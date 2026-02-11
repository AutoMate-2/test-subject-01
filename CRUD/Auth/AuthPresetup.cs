using to_integrations.Config;

namespace to_integrations.CRUD.Auth
{
    public static class AuthPresetup
    {
        public static string ValidLogin => ToIntegrationsEnvironment.AgentId;
        public static string ValidPassword => ToIntegrationsEnvironment.AgentPassword;

        // Boundary test values
        public static string SingleCharLogin => "a";
        public static string SingleCharPassword => "a";
        public static string LongString255 => new string('x', 255);
        public static string SqlInjectionString => "' OR '1'='1'; DROP TABLE users;--";
        public static string XssScriptString => "<script>alert('xss')</script>";

        // Invalid credentials
        public static string InvalidLogin => "invalid_user";
        public static string InvalidPassword => "invalid_password";
    }
}
