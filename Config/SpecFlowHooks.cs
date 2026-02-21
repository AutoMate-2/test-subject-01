using NUnit.Framework;
using Reqnroll;
using to_integrations.HelperMethods;

namespace to_integrations.Config
{
    [Binding]
    public class SpecFlowHooks
    {
        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            AppConfig.Load("Atata.test.json");
            ToIntegrationsEnvironment.Initialize();
            TestContext.Progress.WriteLine($"Base URL: {ToIntegrationsEnvironment.BaseUrl}");

            var agentId = AppConfig.GetValue("AgentId");
            var agentPassword = AppConfig.GetValue("AgentPassword");
            if (!string.IsNullOrEmpty(agentId) && !string.IsNullOrEmpty(agentPassword))
            {
                TokenCache.CachedToken = "agent-credentials-configured";
                TestContext.Progress.WriteLine($"Agent credentials loaded: {agentId}");
            }
            else
            {
                TestContext.Progress.WriteLine("Warning: Agent credentials not configured in Atata.test.json");
            }
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
