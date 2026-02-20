using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using to_integrations.HelperMethods;
using to_integrations.Models;

namespace to_integrations.CRUD.Cities
{
    public class CitiesCrud
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public CitiesCrud()
        {
            _httpClient = new HttpClient();
            _baseUrl = ToIntegrationsEnvironment.BaseUrl;
        }

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
            
            if (!string.IsNullOrEmpty(TokenCache.CachedToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TokenCache.CachedToken);
            }

            var response = await _httpClient.SendAsync(request);
            var statusCode = response.StatusCode;
            
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            CitiesResponse citiesResponse = null;
            if (!string.IsNullOrEmpty(content))
            {
                try
                {
                    citiesResponse = JsonSerializer.Deserialize<CitiesResponse>(content, options);
                }
                catch (JsonException)
                {
                    citiesResponse = new CitiesResponse();
                }
            }

            return (citiesResponse, statusCode);
        }
    }
}
