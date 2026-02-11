using System;
using System.Net.Http;
using System.Threading.Tasks;
using to_integrations.Config;

namespace to_integrations.CRUD.Catalogue.Api
{
    public class CatalogueApi
    {
        public async Task<(int Status, string Body)> GetDestinationCountriesAsync(string queryString = "", string token = "")
        {
            var baseUrl = ToIntegrationsEnvironment.BaseUrl;
            var url = $"{baseUrl}api/Catalogue/GetDestinationCountries{queryString}";

            Console.WriteLine("====== CATALOGUE REQUEST ======");
            Console.WriteLine($"URL: GET {url}");
            
            if (!string.IsNullOrEmpty(token))
            {
                Console.WriteLine($"Authorization: Bearer {token.Substring(0, Math.Min(20, token.Length))}...");
            }
            else
            {
                Console.WriteLine("Authorization: None");
            }
            
            Console.WriteLine("REQUEST BODY: (empty)");
            Console.WriteLine("================================");

            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var res = await client.GetAsync(url);
            return ((int)res.StatusCode, await res.Content.ReadAsStringAsync());
        }
    }
}
