using System.Threading.Tasks;
using NUnit.Framework;
using Reqnroll;
using to_integrations.CRUD.Cities;
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

            // Verify API connectivity by making a test call with configured credentials
            var citiesCrud = new CitiesCrud();
            var result = await citiesCrud.GetCitiesWithStatusAsync();
            Assert.IsTrue((int)result.StatusCode >= 200 && (int)result.StatusCode < 300,
                $"API connectivity check failed with status {result.StatusCode}");
            
            // Mark credentials as validated
            TokenCache.CachedToken = "valid";
            TestContext.Progress.WriteLine("API connectivity verified, credentials are valid");
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
