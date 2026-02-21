using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using to_integrations.HelperMethods;
using to_integrations.Models;

namespace to_integrations.CRUD.HotelPrices
{
    public static class HotelPricesCrud
    {
        public static async Task<(HttpResponseMessage Response, HotelPricesResponse Body, long ElapsedMs)> GetHotelPricesWithStatusAsync(string hotelId)
        {
            using var client = new HttpClient();
            var baseUrl = ToIntegrationsEnvironment.BaseUrl;
            var agentId = AppConfig.GetValue("AgentId") ?? "username";
            var agentPassword = AppConfig.GetValue("AgentPassword") ?? "password";

            var requestUrl = $"{baseUrl}/v3.00/api/hotelprices?agentid={agentId}&agentpassword={agentPassword}&hotelid={hotelId}";

            if (!string.IsNullOrEmpty(TokenCache.CachedToken))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenCache.CachedToken);
            }

            var stopwatch = Stopwatch.StartNew();
            var response = await client.GetAsync(requestUrl);
            stopwatch.Stop();

            HotelPricesResponse body = null;
            var content = await response.Content.ReadAsStringAsync();
            
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                body = JsonSerializer.Deserialize<HotelPricesResponse>(content, options);
            }
            catch (JsonException)
            {
                body = null;
            }

            return (response, body, stopwatch.ElapsedMilliseconds);
        }
    }
}
