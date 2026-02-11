using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Diagnostics;
using to_integrations.CRUD.Catalogue.Api;
using to_integrations.HelperMethods;

namespace to_integrations.CRUD.Catalogue
{
    public class GetArrivalDistrictsCrud
    {
        private readonly GetArrivalDistrictsApi _api = new();
        private JArray? _response;

        public int LastStatus { get; private set; }
        private string _lastUrl = string.Empty;

        public async Task CallAsync(string countryCode, string token)
        {
            _lastUrl = $"/api/Catalogue/GetArrivalDistricts?countryCode={countryCode}";

            var sw = Stopwatch.StartNew();
            var (status, body) = await _api.CallAsync(countryCode, token);
            sw.Stop();

            LastResponse.TimeMs = sw.ElapsedMilliseconds;
            LastStatus = status;

            try
            {
                _response = JArray.Parse(body);
            }
            catch
            {
                _response = null;
            }
        }

        /* ================= BASIC ================= */

        public bool IsArray() => _response != null;

        /* ================= VALIDATIONS ================= */

        public bool EachItemHasFields(string[] fields, string countryCode)
        {
            if (_response == null)
            {
                LogFailures("Structure", new[] { "Response is null" }, countryCode);
                return false;
            }

            var errors = new List<string>();

            foreach (var item in _response)
            {
                foreach (var f in fields)
                {
                    if (item[f] == null)
                        errors.Add($"Item={ItemKey(item)} | Missing field '{f}'");
                }
            }

            if (errors.Any())
            {
                LogFailures("Each item must have required fields", errors, countryCode);
                return false;
            }

            return true;
        }

        public bool ContainsType(string type, string countryCode)
        {
            if (_response == null)
            {
                LogFailures("Contains type", new[] { "Response is null" }, countryCode);
                return false;
            }

            if (!_response.Any(x => x["type"]?.ToString() == type))
            {
                LogFailures(
                    "Contains type",
                    new[] { $"No items with type='{type}' found" },
                    countryCode
                );
                return false;
            }

            return true;
        }

        public bool DistrictParentsReferenceCities(string countryCode)
        {
            if (_response == null)
            {
                LogFailures("District parent reference", new[] { "Response is null" }, countryCode);
                return false;
            }

            var cityValues = _response
                .Where(x => x["type"]?.ToString() == "city")
                .Select(x => x["value"]?.ToString())
                .ToHashSet();

            var errors = new List<string>();

            foreach (var d in _response.Where(x => x["type"]?.ToString() == "district"))
            {
                var parent = d["parent"]?.ToString();
                if (!cityValues.Contains(parent))
                {
                    errors.Add(
                        $"District={ItemKey(d)} | Parent '{parent}' does not reference any city"
                    );
                }
            }

            if (errors.Any())
            {
                LogFailures("District parent must reference valid city", errors, countryCode);
                return false;
            }

            return true;
        }

        public bool ContainsCity(string city)
        {
            if (_response == null) return false;

            return _response.Any(x =>
                x["type"]?.ToString() == "city" &&
                x["label"]?.ToString() == city
            );
        }

        /* ================= LOGGING ================= */

        internal void LogFailures(string rule, IEnumerable<string> errors, string countryCode)
        {
            TestContext.WriteLine("========================================");
            TestContext.WriteLine("[FAIL][GetArrivalDistricts]");
            TestContext.WriteLine($"countryCode : {countryCode}");
            TestContext.WriteLine($"status      : {LastStatus}");
            TestContext.WriteLine($"url         : {_lastUrl}");
            TestContext.WriteLine($"rule        : {rule}");
            TestContext.WriteLine("failures:");

            int i = 0;
            foreach (var e in errors)
                TestContext.WriteLine($"  {++i}. {e}");

            TestContext.WriteLine("========================================");
        }

        private string ItemKey(JToken item)
        {
            return $"type={item["type"]}, label='{item["label"]}', value={item["value"]}";
        }
    }
}
