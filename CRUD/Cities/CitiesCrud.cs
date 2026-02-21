using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
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

            if (!string.IsNullOrEmpty(TokenCache.CachedToken))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", TokenCache.CachedToken);
            }

            var sw = Stopwatch.StartNew();
            var response = await client.GetAsync("/api/cities");
            sw.Stop();

            var content = await response.Content.ReadAsStringAsync();
            var body = JsonSerializer.Deserialize<CitiesResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return (response, body, sw.ElapsedMilliseconds);
        }
    }
}
