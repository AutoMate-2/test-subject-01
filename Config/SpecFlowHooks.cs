using System.Threading.Tasks;
using NUnit.Framework;
using TechTalk.SpecFlow;
using to_integrations.CRUD.Auth;
using to_integrations.HelperMethods;

namespace to_integrations.Config
{
    [Binding]
    public class SpecFlowHooks
    {
        [BeforeTestRun]
        public static async Task BeforeTestRun()
        {
            AppConfig.Load("Atata.test.json");
            ToIntegrationsEnvironment.Initialize();
            TestContext.Progress.WriteLine($"Base URL: {ToIntegrationsEnvironment.BaseUrl}");

            var authCrud = new AuthCrud();
            TokenCache.CachedToken = await authCrud.LoginAsync();
            TestContext.Progress.WriteLine("Authentication completed, token cached");
        }

        [BeforeScenario]
        public void BeforeScenario(ScenarioContext scenarioContext)
        {
            TestContext.Progress.WriteLine($"Starting scenario: {scenarioContext.ScenarioInfo.Title}");
        }

        [AfterScenario]
        public void AfterScenario(ScenarioContext scenarioContext)
        {
            TestContext.Progress.WriteLine($"Completed scenario: {scenarioContext.ScenarioInfo.Title}");
        }

        [BeforeScenario("@noauth")]
        public void BeforeNoAuthScenario()
        {
            TokenCache.CachedToken = null;
            TestContext.Progress.WriteLine("Cleared authentication token for @noauth scenario");
        }
    }
}
