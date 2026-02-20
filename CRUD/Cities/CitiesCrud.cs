using System;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using to_integrations.HelperMethods;

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

        public async Task<HttpResponseMessage> GetCitiesAsync(string agentId, string agentPassword)
        {
            var requestUrl = $"{_baseUrl}/v3.00/api/Cities?agentid={Uri.EscapeDataString(agentId)}&agentpassword={Uri.EscapeDataString(agentPassword)}";
            
            TestContext.Progress.WriteLine($"Sending GET request to: {requestUrl}");
            
            var response = await _httpClient.GetAsync(requestUrl);
            
            return response;
        }
    }
}
