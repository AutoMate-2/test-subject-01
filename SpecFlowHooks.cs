using NUnit.Framework;
using TechTalk.SpecFlow;
using to_integrations.CRUD.Auth;
using to_integrations.CRUD.Catalogue;
using to_integrations.HelperMethods;

namespace to_integrations.Config
{
    [Binding]
    public static class SpecFlowHooks
    {
        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            AppConfig.Load("Atata.test.json");
            ToIntegrationsEnvironment.Initialize();

            TestContext.Progress.WriteLine($"Base URL: {ToIntegrationsEnvironment.BaseUrl}");

            CacheAuthenticationToken();
        }

        [BeforeScenario]
        public static void BeforeScenario()
        {
            TestContext.Progress.WriteLine($"Running scenario against: {ToIntegrationsEnvironment.BaseUrl}");
        }

        [BeforeScenario("@noauth")]
        public static void BeforeUnauthenticatedScenario()
        {
            TestContext.Progress.WriteLine("Overriding authentication: NO AUTH scenario");

            TokenCache.CachedToken = string.Empty;
            CataloguePresetup.InitializeTokens(string.Empty);
        }

        private static void CacheAuthenticationToken()
        {
            try
            {
                TestContext.Progress.WriteLine("Authenticating to get token...");
                
                var authCrud = new AuthCrud();
                var authTask = authCrud.LoginAsync(AuthPresetup.ValidLogin, AuthPresetup.ValidPassword);
                
                var completed = authTask.Wait(TimeSpan.FromSeconds(15));
                if (!completed)
                {
                    TestContext.Progress.WriteLine("Warning: Auth timed out after 15 seconds");
                    return;
                }

                var token = authCrud.GetAccessToken();
                if (!string.IsNullOrEmpty(token))
                {
                    TokenCache.CachedToken = token;
                    CataloguePresetup.InitializeTokens(token);
                    TestContext.Progress.WriteLine($"Token cached: {token.Substring(0, Math.Min(30, token.Length))}...");
                }
                else
                {
                    TestContext.Progress.WriteLine($"Warning: Empty token");
                }
            }
            catch (Exception ex)
            {
                TestContext.Progress.WriteLine($"Auth error: {ex.Message}");
            }
        }
    }
}
