using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using to_integrations.Config;

namespace to_integrations.CRUD.Auth.Api
{
    public class AuthApi
    {
        private string BeautifyJson(string json)
        {
            try
            {
                var parsedJson = JToken.Parse(json);
                return parsedJson.ToString(Formatting.Indented);
            }
            catch
            {
                return json;
            }
        }

        public async Task<(int Status, string Body)> LoginAsync(string login, string password)
        {
            var baseUrl = ToIntegrationsEnvironment.BaseUrl;
            var url = $"{baseUrl}api/Auth/Auth";

            var requestBody = new
            {
                login = login,
                password = password
            };

            var json = JsonConvert.SerializeObject(requestBody);

            Console.WriteLine("====== AUTH REQUEST ======");
            Console.WriteLine($"URL: POST {url}");
            Console.WriteLine("REQUEST BODY:");
            Console.WriteLine(BeautifyJson(json));
            Console.WriteLine("==========================");

            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var res = await client.PostAsync(url, content);

            return ((int)res.StatusCode, await res.Content.ReadAsStringAsync());
        }

        public async Task<(int Status, string Body)> LoginWithoutBodyAsync()
        {
            var baseUrl = ToIntegrationsEnvironment.BaseUrl;
            var url = $"{baseUrl}api/Auth/Auth";

            Console.WriteLine("====== AUTH REQUEST (NO BODY) ======");
            Console.WriteLine($"URL: POST {url}");
            Console.WriteLine("REQUEST BODY: (empty)");
            Console.WriteLine("====================================");

            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            var content = new StringContent("", Encoding.UTF8, "application/json");
            var res = await client.PostAsync(url, content);

            return ((int)res.StatusCode, await res.Content.ReadAsStringAsync());
        }
    }
}
