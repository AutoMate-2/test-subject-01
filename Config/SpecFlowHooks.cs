using System;
using NUnit.Framework;
using TechTalk.SpecFlow;
using to_integrations.HelperMethods;

namespace to_integrations.Config
{
    [Binding]
    public static class SpecFlowHooks
    {
        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            TestContext.Progress.WriteLine("Initializing test run...");
            AppConfig.Load("Atata.test.json");
            ToIntegrationsEnvironment.Initialize();
            TestContext.Progress.WriteLine($"Base URL: {ToIntegrationsEnvironment.BaseUrl}");
        }

        [BeforeScenario]
        public static void BeforeScenario(ScenarioContext scenarioContext)
        {
            TestContext.Progress.WriteLine($"Starting scenario: {scenarioContext.ScenarioInfo.Title}");
        }

        [AfterScenario]
        public static void AfterScenario(ScenarioContext scenarioContext)
        {
            TestContext.Progress.WriteLine($"Completed scenario: {scenarioContext.ScenarioInfo.Title}");
        }

        [BeforeScenario("@noauth")]
        public static void BeforeUnauthenticatedScenario()
        {
            TokenCache.CachedToken = string.Empty;
            TestContext.Progress.WriteLine("Cleared authentication token for @noauth scenario");
        }
    }
}
