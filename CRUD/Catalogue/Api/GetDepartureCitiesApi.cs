using System.Net.Http;
using System.Threading.Tasks;
using to_integrations.Config;

namespace to_integrations.CRUD.Catalogue.Api
{
    internal class GetDepartureCitiesApi
    {
        public async Task<(int Status, string Body)> GetDepartureCities(
            string countryCode,
            string token = "")
        {
            var baseUrl = ToIntegrationsEnvironment.BaseUrl;

            var url = string.IsNullOrWhiteSpace(countryCode)
                ? $"{baseUrl.TrimEnd('/')}/api/Catalogue/GetDepartureCities"
                : $"{baseUrl.TrimEnd('/')}/api/Catalogue/GetDepartureCities?countryCode={countryCode}";

            var client = new HttpClient();

            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var response = await client.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();

            var status = (int)response.StatusCode;

            System.Console.WriteLine($"GET {url} - Status: {status}");
            System.Console.WriteLine(
     Newtonsoft.Json.Linq.JToken.Parse(body)
         .ToString(Newtonsoft.Json.Formatting.Indented));


            return (status, body);
        }
    }
}
