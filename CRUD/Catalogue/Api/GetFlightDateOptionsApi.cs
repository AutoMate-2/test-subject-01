using System.Net.Http;
using System.Threading.Tasks;
using to_integrations.Config;

namespace to_integrations.CRUD.Catalogue.Api
{
    internal class GetFlightDateOptionsApi
    {
        public async Task<(int Status, string Body)> CallAsync(
            string cityUid,
            string countryCode,
            bool isFrom,
            int month,
            int year,
            string token)
        {
            var baseUrl = ToIntegrationsEnvironment.BaseUrl.TrimEnd('/');

            var url =
                $"{baseUrl}/api/Catalogue/GetFlightDateOptions" +
                $"?CityUID={cityUid}" +
                $"&CountryCode={countryCode}" +
                $"&IsFrom={isFrom.ToString().ToLower()}" +
                $"&Month={month}" +
                $"&Year={year}";

            using var client = new HttpClient();

            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var response = await client.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();

            return ((int)response.StatusCode, body);
        }
    }
}
