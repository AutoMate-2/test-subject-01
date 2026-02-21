using to_integrations.CRUD.Cities;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using Reqnroll;
using to_integrations.CRUD.Cities;
using to_integrations.HelperMethods;
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
            var result = await CitiesCrud.GetCitiesWithStatusAsync();
            Assert.IsTrue((int)result.Response.StatusCode >= 200 && (int)result.Response.StatusCode < 300,
                $"API connectivity check failed with status {result.Response.StatusCode}");
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
