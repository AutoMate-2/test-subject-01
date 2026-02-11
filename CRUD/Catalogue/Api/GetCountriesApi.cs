using System.Net.Http;
using System.Threading.Tasks;
using to_integrations.Config;

namespace to_integrations.CRUD.Catalogue.Api
{
    internal class GetCountriesApi
    {
        public async Task<(int Status, string Body)> GetCountries(string token = "")
        {
            var baseUrl = ToIntegrationsEnvironment.BaseUrl;
            var url = $"{baseUrl.TrimEnd('/')}/api/Catalogue/GetCountries";


            var client = new HttpClient();

            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var response = await client.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();

            var status = (int)response.StatusCode;

            System.Console.WriteLine($"GET {url} - Status: {status}");
            System.Console.WriteLine(body);

            return (status, body);
        }
    }
}
