using to_integrations.Config;

namespace to_integrations.Presetup
{
    public static class CountryCodesPresetup
    {
        public static string[] All =>
            AppConfig.Current?.CountryCodes
            ?? throw new Exception("CountryCodes missing in AppConfig");
    }
}
