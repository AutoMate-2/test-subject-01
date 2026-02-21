<<<<<<< HEAD
using NUnit.Framework;
using Reqnroll;
=======
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using TechTalk.SpecFlow;
>>>>>>> ca0b864a037c063f4f0ffcb95fc6b5dcb30b07f2
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
<<<<<<< HEAD
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
=======
            await CacheAuthenticationTokenAsync();
            TestContext.Progress.WriteLine("Test run initialized. Base URL: " + ToIntegrationsEnvironment.BaseUrl);
>>>>>>> ca0b864a037c063f4f0ffcb95fc6b5dcb30b07f2
        }

        [BeforeScenario]
        public void BeforeScenario(ScenarioContext scenarioContext)
        {
            TestContext.Progress.WriteLine($"Starting scenario: {scenarioContext.ScenarioInfo.Title}");
        }

        [AfterScenario]
        public void AfterScenario(ScenarioContext scenarioContext)
        {
            TestContext.Progress.WriteLine($"Finished scenario: {scenarioContext.ScenarioInfo.Title}");
        }

        [BeforeScenario("@noauth")]
        public void BeforeNoAuthScenario()
        {
            TokenCache.CachedToken = null;
        }

        private static async Task CacheAuthenticationTokenAsync()
        {
            try
            {
                var agentId = AppConfig.GetValue("AgentId") ?? "username";
                var agentPassword = AppConfig.GetValue("AgentPassword") ?? "password";

                using var client = new HttpClient();
                client.BaseAddress = new Uri(ToIntegrationsEnvironment.BaseUrl);

                var payload = JsonSerializer.Serialize(new
                {
                    AgentId = agentId,
                    AgentPassword = agentPassword
                });

                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("/api/authenticate", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(responseBody);
                if (doc.RootElement.TryGetProperty("Token", out var tokenElement))
                {
                    TokenCache.CachedToken = tokenElement.GetString();
                }
                else if (doc.RootElement.TryGetProperty("token", out var tokenElementLower))
                {
                    TokenCache.CachedToken = tokenElementLower.GetString();
                }

                TestContext.Progress.WriteLine("Authentication token cached successfully.");
            }
            catch (Exception ex)
            {
                TestContext.Progress.WriteLine($"Warning: Could not cache auth token: {ex.Message}");
            }
        }
    }
}
