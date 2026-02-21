using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using to_integrations.HelperMethods;
using to_integrations.Models;

namespace to_integrations.CRUD.Cities
{
    public static class CitiesCrud
    {
        public static async Task<(HttpResponseMessage Response, CitiesResponse Body, long ElapsedMs)> GetCitiesWithStatusAsync()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(ToIntegrationsEnvironment.BaseUrl);

            var agentId = AppConfig.GetValue("AgentId") ?? "username";
            var agentPassword = AppConfig.GetValue("AgentPassword") ?? "password";

            var requestUrl = $"/v3.00/api/Cities?agentid={Uri.EscapeDataString(agentId)}&agentpassword={Uri.EscapeDataString(agentPassword)}";

            var stopwatch = Stopwatch.StartNew();
            var response = await client.GetAsync(requestUrl);
            stopwatch.Stop();

            CitiesResponse body = null;
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                try
                {
                    body = JsonSerializer.Deserialize<CitiesResponse>(content);
                }
                catch (JsonException)
                {
                    body = null;
                }
            }

            return (response, body, stopwatch.ElapsedMilliseconds);
        }
    }
}
