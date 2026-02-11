using System.Net.Http;
using System.Threading.Tasks;
using to_integrations.Config;

namespace to_integrations.CRUD.Catalogue.Api
{
    internal class GetArrivalDistrictsApi
    {
        public async Task<(int Status, string Body)> CallAsync(string countryCode, string token)
        {
            var baseUrl = ToIntegrationsEnvironment.BaseUrl;
            var url = $"{baseUrl.TrimEnd('/')}/api/Catalogue/GetArrivalDistricts?countryCode={countryCode}";

            using var client = new HttpClient();

            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var response = await client.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();

    //        System.Console.WriteLine($"GET {url} - Status: {(int)response.StatusCode}");
    //        System.Console.WriteLine(
    //Newtonsoft.Json.Linq.JToken.Parse(body)
    //    .ToString(Newtonsoft.Json.Formatting.Indented));

            return ((int)response.StatusCode, body);
        }
    }
}
