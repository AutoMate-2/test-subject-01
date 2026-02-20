using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using to_integrations.CRUD.Cities;
using to_integrations.HelperMethods;

namespace to_integrations.CRUD.Auth
{
    public class AuthCrud
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public AuthCrud()
        {
            _httpClient = new HttpClient();
            _baseUrl = ToIntegrationsEnvironment.BaseUrl;
        }

        public async Task<string> LoginAsync()
        {
            var presetup = new CitiesPresetup();
            var loginRequest = new
            {
                username = presetup.ValidAgentId,
                password = presetup.ValidAgentPassword
            };

            var content = new StringContent(
                JsonSerializer.Serialize(loginRequest),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/api/auth/login", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseContent, options);

            return authResponse?.Token;
        }
    }

    public class AuthResponse
    {
        public string Token { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
    }
}
