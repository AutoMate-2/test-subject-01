using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using to_integrations.HelperMethods;
using ToIntegrations.Models;

namespace ToIntegrations.CRUD.Hotels
{
    public static class HotelsCrud
    {
        public static async Task<(HttpResponseMessage Response, HotelsResponse Body, long ElapsedMs)> GetHotelsWithStatusAsync()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(ToIntegrationsEnvironment.BaseUrl);

            var agentId = AppConfig.GetValue("AgentId") ?? "username";
            var agentPassword = AppConfig.GetValue("AgentPassword") ?? "password";

            var requestUrl = $"/v3.00/api/Hotels?agentid={Uri.EscapeDataString(agentId)}&agentpassword={Uri.EscapeDataString(agentPassword)}";

            var stopwatch = Stopwatch.StartNew();
            var response = await client.GetAsync(requestUrl);
            stopwatch.Stop();

            HotelsResponse body = null;
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                try
                {
                    body = JsonSerializer.Deserialize<HotelsResponse>(content);
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
