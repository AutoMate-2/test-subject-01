using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using to_integrations.HelperMethods;
using ToIntegrations.Models;

namespace ToIntegrations.CRUD.Areas
{
    public static class AreasCrud
    {
        public static async Task<(HttpResponseMessage Response, AreasResponse Body, long ElapsedMs)> GetAreasWithStatusAsync()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(ToIntegrationsEnvironment.BaseUrl);

            var agentId = AppConfig.GetValue("AgentId") ?? "username";
            var agentPassword = AppConfig.GetValue("AgentPassword") ?? "password";

            var requestUrl = $"/v3.00/api/Areas?agentid={Uri.EscapeDataString(agentId)}&agentpassword={Uri.EscapeDataString(agentPassword)}";

            var stopwatch = Stopwatch.StartNew();
            var response = await client.GetAsync(requestUrl);
            stopwatch.Stop();

            AreasResponse body = null;
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                try
                {
                    body = JsonSerializer.Deserialize<AreasResponse>(content);
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
