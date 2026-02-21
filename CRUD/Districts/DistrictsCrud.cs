using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using to_integrations.HelperMethods;
using to_integrations.Models;

namespace to_integrations.CRUD.Districts
{
    public static class DistrictsCrud
    {
        public static async Task<(HttpResponseMessage Response, DistrictsResponse Body, long ElapsedMs)> GetDistrictsWithStatusAsync()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(ToIntegrationsEnvironment.BaseUrl);

            if (!string.IsNullOrEmpty(TokenCache.CachedToken))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", TokenCache.CachedToken);
            }

            var sw = Stopwatch.StartNew();
            var response = await client.GetAsync("/api/districts");
            sw.Stop();

            var content = await response.Content.ReadAsStringAsync();
            var body = JsonSerializer.Deserialize<DistrictsResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return (response, body, sw.ElapsedMilliseconds);
        }
    }
}
