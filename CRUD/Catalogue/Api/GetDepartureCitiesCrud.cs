using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Diagnostics;
using System.Globalization;
using to_integrations.CRUD.Catalogue.Api;
using to_integrations.HelperMethods;

namespace to_integrations.CRUD.Catalogue
{
    public class GetDepartureCitiesCrud
    {
        private readonly GetDepartureCitiesApi _api = new();
        private JArray? _cities;

        public int LastStatus { get; private set; }
        public string LastBody { get; private set; } = "";

        private string _lastUrl = string.Empty;

        public async Task CallGetDepartureCities(string countryCode, string token = "")
        {
            // Build URL for reliable logging
            _lastUrl = string.IsNullOrWhiteSpace(countryCode)
                ? "/api/Catalogue/GetDepartureCities"
                : $"/api/Catalogue/GetDepartureCities?countryCode={countryCode}";

            var sw = Stopwatch.StartNew();

            var (status, body) = await _api.GetDepartureCities(countryCode, token);

            sw.Stop();
            LastResponse.TimeMs = sw.ElapsedMilliseconds;

            LastStatus = status;
            LastBody = body;

            ParseResponse(body);
        }

        private void ParseResponse(string body)
        {
            try
            {
                _cities = JArray.Parse(body);
            }
            catch
            {
                _cities = null;
            }
        }

        public bool IsArray() => _cities != null;

        public bool IsEmptyArray() => _cities != null && !_cities.Any();

        public bool HasCity(string cityName)
            => _cities != null &&
               _cities.Any(c => c["cityName"]?.ToString() == cityName);

        public bool EachCityHasFields(string[] fields)
        {
            if (_cities == null)
            {
                LogFailures("Required fields", new[] { "Response is null or not an array" }, countryCode: "");
                return false;
            }

            var errors = new List<string>();

            foreach (var city in _cities)
            {
                if (city is not JObject obj)
                {
                    errors.Add($"City={CityKey(city)} | Invalid item (not a JSON object)");
                    continue;
                }

                foreach (var field in fields)
                {
                    if (string.IsNullOrWhiteSpace(field))
                        continue;

                    if (obj[field] == null)
                        errors.Add($"City={CityKey(city)} | Missing field '{field}'");
                }
            }

            if (errors.Any())
            {
                LogFailures("Each city should have required fields", errors, countryCode: "");
                return false;
            }

            return true;
        }

        // NOTE: signature extended only to include countryCode for logging; validation rule unchanged.
        public bool EachCityUidIsValidGuid(string countryCode)
        {
            if (_cities == null)
            {
                LogFailures("cityUID GUID", new[] { "Response is null or not an array" }, countryCode);
                return false;
            }

            var errors = new List<string>();

            foreach (var city in _cities)
            {
                var value = city?["cityUID"]?.ToString();

                if (!Guid.TryParse(value, out _))
                    errors.Add($"City={CityKey(city)} | Invalid cityUID GUID: '{value ?? "null"}'");
            }

            if (errors.Any())
            {
                LogFailures("cityUID must be a valid GUID", errors, countryCode);
                return false;
            }

            return true;
        }

        // NOTE: signature extended only to include countryCode for logging; validation rule unchanged.
        public bool EachTimeZoneOffsetValid(string countryCode)
        {
            if (_cities == null)
            {
                LogFailures("timeZoneOffset range", new[] { "Response is null or not an array" }, countryCode);
                return false;
            }

            var errors = new List<string>();

            foreach (var city in _cities)
            {
                var raw = city?["timeZoneOffset"]?.ToString();

                // Keep your validation: must be int, between -12 and 14
                if (!int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v) ||
                    v < -12 || v > 14)
                {
                    errors.Add($"City={CityKey(city)} | Invalid timeZoneOffset: '{raw ?? "null"}' (allowed: -12..14)");
                }
            }

            if (errors.Any())
            {
                LogFailures("timeZoneOffset must be integer and within -12..14", errors, countryCode);
                return false;
            }

            return true;
        }

        // ---------------------------------------------------------
        // Logging helpers (added; no behavior changes)
        // ---------------------------------------------------------

        internal void LogFailures(string rule, IEnumerable<string> errors, string countryCode)
        {
            TestContext.WriteLine("========================================");
            TestContext.WriteLine("[FAIL][GetDepartureCities]");
            if (!string.IsNullOrWhiteSpace(countryCode))
                TestContext.WriteLine($"countryCode : {countryCode}");
            TestContext.WriteLine($"status      : {LastStatus}");
            TestContext.WriteLine($"url         : {_lastUrl}");
            TestContext.WriteLine($"rule        : {rule}");
            TestContext.WriteLine("failures:");

            int count = 0;
            foreach (var e in errors)
            {
                count++;
                TestContext.WriteLine($"  {count}. {e}");
            }

            TestContext.WriteLine("========================================");
        }

        private string CityKey(JToken? city)
        {
            var cityUid = city?["cityUID"]?.ToString();
            var cityName = city?["cityName"]?.ToString();
            var countryCode = city?["countryCode"]?.ToString();

            return $"cityUID={cityUid ?? "?"}, cityName='{cityName ?? "?"}', countryCode={countryCode ?? "?"}";
        }
    }
}
