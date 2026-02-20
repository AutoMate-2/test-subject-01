using System;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using to_integrations.HelperMethods;

namespace to_integrations.CRUD.Districts
{
    public class DistrictsCrud
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public DistrictsCrud()
        {
            _httpClient = new HttpClient();
            _baseUrl = ToIntegrationsEnvironment.BaseUrl;
        }

        public async Task<HttpResponseMessage> GetDistrictsAsync(string agentId, string agentPassword)
        {
            var requestUrl = $"{_baseUrl}/v3.00/api/Districts?agentid={Uri.EscapeDataString(agentId)}&agentpassword={Uri.EscapeDataString(agentPassword)}";
            
            TestContext.Progress.WriteLine($"Sending GET request to: {requestUrl}");
            
            var response = await _httpClient.GetAsync(requestUrl);
            
            return response;
        }
    }
}
