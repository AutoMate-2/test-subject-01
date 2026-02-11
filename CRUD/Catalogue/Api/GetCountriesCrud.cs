using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using to_integrations.HelperMethods;

namespace to_integrations.CRUD.Catalogue.Api
{
    public class GetCountriesCrud
    {
        private readonly GetCountriesApi _api = new GetCountriesApi();
        private JObject? _response;

        public int LastStatus { get; private set; }
        public string LastBody { get; private set; } = "";

        public async Task CallGetCountriesApi(string token = "")
        {
            var sw = Stopwatch.StartNew();

            var (status, body) = await _api.GetCountries(token);

            sw.Stop();
            LastResponse.TimeMs = sw.ElapsedMilliseconds;

            LastStatus = status;
            LastBody = body;

            ParseResponse(body);
            LogResponse(status, body);
        }

        private void ParseResponse(string body)
        {
            try
            {
                _response = JObject.Parse(body);
            }
            catch
            {
                _response = null;
            }
        }

        private void LogResponse(int status, string body)
        {
            System.Console.WriteLine("====== CATALOGUE RESPONSE ======");
            System.Console.WriteLine($"Status Code: {status}");
            System.Console.WriteLine("Response Body:");

            try
            {
                var pretty = JToken.Parse(body).ToString(Newtonsoft.Json.Formatting.Indented);
                System.Console.WriteLine(pretty);
            }
            catch
            {
                System.Console.WriteLine(body);
            }

            System.Console.WriteLine("================================");
        }

        // -------- Helper methods (NO ASSERTS) --------

        public bool HasCountriesArray()
        {
            return _response?["countries"] is JArray;
        }

        public bool HasErrorFalse()
        {
            return _response?["error"] != null &&
                   _response["error"]!.Type == JTokenType.Boolean &&
                   _response["error"]!.Value<bool>() == false;
        }

        public bool HasCountryCode(string code)
        {
            var countries = _response?["countries"] as JArray;
            if (countries == null) return false;

            return countries.Any(c =>
                c["countryCode"] != null &&
                c["countryCode"]!.ToString() == code);
        }

        public bool ArePhoneCodesValid()
        {
            var countries = _response?["countries"] as JArray;
            if (countries == null)
            {
                System.Console.WriteLine("No countries array found.");
                return false;
            }

            bool allValid = true;

            foreach (var c in countries)
            {
                var countryCode = c["countryCode"]?.ToString();
                var phoneCode = c["phoneCode"]?.ToString();

                if (string.IsNullOrWhiteSpace(phoneCode))
                {
                    System.Console.WriteLine(
                        $"INVALID phoneCode: empty | countryCode={countryCode}"
                    );
                    allValid = false;
                    continue;
                }

                // allow digits, space, dash, comma
                bool isValid = phoneCode.All(ch =>
                    char.IsDigit(ch) || ch == '-' || ch == ',' || ch == ' '
                );

                if (!isValid)
                {
                    System.Console.WriteLine(
                        $"INVALID phoneCode '{phoneCode}' | countryCode={countryCode}"
                    );
                    allValid = false;
                }
                else
                {
                    System.Console.WriteLine(
                        $" VALID phoneCode '{phoneCode}' | countryCode={countryCode}"
                    );
                }
            }

            return allValid;
        }

    }
}
