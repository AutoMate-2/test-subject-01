using System.Net.Http;
using System.Threading.Tasks;
using to_integrations.Config;

namespace to_integrations.CRUD.Catalogue.Api
{
    internal class GetHotelsApi
    {
        public async Task<(int Status, string Body)> CallAsync(
            string countryCode,
            string token)
        {
            var baseUrl = ToIntegrationsEnvironment.BaseUrl.TrimEnd('/');
            var url = $"{baseUrl}/api/Catalogue/GetHotels?countryCode={countryCode}";

            using var client = new HttpClient();

            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var response = await client.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();

            //System.Console.WriteLine($"GET {url} - Status: {(int)response.StatusCode}");
            //System.Console.WriteLine(
            //    Newtonsoft.Json.Linq.JToken.Parse(body)
            //        .ToString(Newtonsoft.Json.Formatting.Indented)
            //);

            return ((int)response.StatusCode, body);
        }
    }
}
