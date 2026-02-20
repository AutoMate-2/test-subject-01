namespace to_integrations.HelperMethods
{
    public static class ToIntegrationsEnvironment
    {
        public static string BaseUrl { get; private set; }

        public static void Initialize()
        {
            BaseUrl = AppConfig.GetValue("BaseUrl") ?? "https://restapi.rustaronline.com";
        }
    }
}
