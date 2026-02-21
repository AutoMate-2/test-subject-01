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

<<<<<<< HEAD
        public async Task<CitiesResponse> GetCitiesAsync()
        {
            var result = await GetCitiesWithStatusAsync();
            return result.Response;
        }

        public async Task<(CitiesResponse Response, HttpStatusCode StatusCode)> GetCitiesWithStatusAsync()
        {
            var presetup = new CitiesPresetup();
            var agentId = presetup.ValidAgentId;
            var agentPassword = presetup.ValidAgentPassword;
            
            var requestUrl = $"{_baseUrl}/v3.00/api/Cities?agentid={Uri.EscapeDataString(agentId)}&agentpassword={Uri.EscapeDataString(agentPassword)}";
            
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
=======
            if (!string.IsNullOrEmpty(TokenCache.CachedToken))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", TokenCache.CachedToken);
            }
>>>>>>> fe59efb2efbd46ad04271a7b350e0108db3311ef

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
