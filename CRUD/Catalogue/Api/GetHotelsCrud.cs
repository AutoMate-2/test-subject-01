using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Diagnostics;
using System.Globalization;
using to_integrations.CRUD.Catalogue.Api;
using to_integrations.Config;

namespace to_integrations.CRUD.Catalogue
{
    public class GetHotelsCrud
    {
        private readonly GetHotelsApi _api = new();
        private JArray? _response;

        public int LastStatus { get; private set; }

        private string _currentCountryCode = string.Empty;
        private string _lastUrl = string.Empty;

        public async Task CallAsync(string countryCode, string token)
        {
            _currentCountryCode = countryCode;

            // Build URL here for logging (so we can show it even if API only prints to console).
            var baseUrl = ToIntegrationsEnvironment.BaseUrl?.TrimEnd('/') ?? string.Empty;
            _lastUrl = string.IsNullOrWhiteSpace(countryCode)
                ? $"{baseUrl}/api/Catalogue/GetHotels"
                : $"{baseUrl}/api/Catalogue/GetHotels?countryCode={countryCode}";

            var sw = Stopwatch.StartNew();
            var (status, body) = await _api.CallAsync(countryCode, token);
            sw.Stop();

            LastStatus = status;

            try
            {
                _response = JArray.Parse(body);
            }
            catch
            {
                _response = null;
                LogFailures("Response parsing", new[]
                {
                    "Response is not a valid JSON array"
                });
            }
        }

        public bool IsArray() => _response != null;

        public bool IsEmptyArray() => _response != null && !_response.Any();

        public bool EachHotelHasFields(IEnumerable<string> requiredFields)
        {
            if (_response == null || _response.Type != JTokenType.Array)
            {
                LogFailures("Required fields", new[] { "Response is not a JSON array (or is null)" });
                return false;
            }

            var required = requiredFields
                .Where(f => !string.IsNullOrWhiteSpace(f))
                .Select(f => f.Trim().ToLowerInvariant())
                .ToArray();

            var errors = new List<string>();

            foreach (var hotel in _response)
            {
                if (hotel is not JObject obj)
                {
                    errors.Add($"Hotel={HotelKey(hotel)} | Invalid item (not a JSON object)");
                    continue;
                }

                var keys = obj.Properties()
                              .Select(p => p.Name.ToLowerInvariant())
                              .ToHashSet();

                foreach (var field in required)
                {
                    if (!keys.Contains(field))
                        errors.Add($"Hotel={HotelKey(hotel)} | Missing field '{field}'");
                }
            }

            if (errors.Any())
            {
                LogFailures("Each hotel should have required fields", errors);
                return false;
            }

            return true;
        }

        public bool EachHotelCodeIsGuid()
        {
            if (_response == null)
            {
                LogFailures("hotelCode GUID", new[] { "Response is null. Did the call succeed and parse?" });
                return false;
            }

            var errors = new List<string>();

            foreach (var hotel in _response)
            {
                var value = hotel?["hotelCode"]?.ToString();

                if (!Guid.TryParse(value, out _))
                    errors.Add($"Hotel={HotelKey(hotel)} | Invalid hotelCode GUID: '{value ?? "null"}'");
            }

            if (errors.Any())
            {
                LogFailures("hotelCode must be a valid GUID", errors);
                return false;
            }

            return true;
        }

        public bool EachHotelClassValid()
        {
            if (_response == null)
            {
                LogFailures("hotelClass allowed", new[] { "Response is null. Did the call succeed and parse?" });
                return false;
            }

            var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "1", "2", "3", "4", "5", "6" };
            var errors = new List<string>();

            foreach (var hotel in _response)
            {
                var value = hotel?["hotelClass"]?.ToString();

                if (string.IsNullOrWhiteSpace(value) || !allowed.Contains(value.Trim()))
                    errors.Add($"Hotel={HotelKey(hotel)} | Invalid hotelClass: '{value ?? "null"}' (allowed: 1..5)");
            }

            if (errors.Any())
            {
                LogFailures("hotelClass must be in allowed set", errors);
                return false;
            }

            return true;
        }

        public bool EachHotelTypeValid()
        {
            if (_response == null)
            {
                LogFailures("hotelType allowed", new[] { "Response is null. Did the call succeed and parse?" });
                return false;
            }

            var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "CityHotel",
                "BeachHotel",
                "SecondLineBeach"
            };

            var errors = new List<string>();

            foreach (var hotel in _response)
            {
                var value = hotel?["hotelType"]?.ToString();

                if (string.IsNullOrWhiteSpace(value) || !allowed.Contains(value.Trim()))
                    errors.Add($"Hotel={HotelKey(hotel)} | Invalid hotelType: '{value ?? "null"}' (allowed: CityHotel, BeachHotel, SecondLineBeach)");
            }

            if (errors.Any())
            {
                LogFailures("hotelType must be in allowed set", errors);
                return false;
            }

            return true;
        }

        public bool EachBooleanField(string field)
        {
            if (_response == null)
            {
                LogFailures($"{field} boolean", new[] { "Response is null. Did the call succeed and parse?" });
                return false;
            }

            var errors = new List<string>();

            foreach (var hotel in _response)
            {
                var token = hotel?[field];

                if (token == null)
                {
                    errors.Add($"Hotel={HotelKey(hotel)} | Missing boolean field '{field}'");
                    continue;
                }

                if (token.Type != JTokenType.Boolean)
                    errors.Add($"Hotel={HotelKey(hotel)} | Field '{field}' is not boolean (actual type: {token.Type})");
            }

            if (errors.Any())
            {
                LogFailures($"Field '{field}' should be boolean", errors);
                return false;
            }

            return true;
        }

        public bool CoordinatesValid()
        {
            if (_response == null)
            {
                LogFailures("Coordinates format/range", new[] { "Response is null. Did the call succeed and parse?" });
                return false;
            }

            var errors = new List<string>();

            foreach (var hotel in _response)
            {
                var latStr = hotel?["latitude"]?.ToString();
                var lonStr = hotel?["longitude"]?.ToString();

                if (string.IsNullOrWhiteSpace(latStr) || string.IsNullOrWhiteSpace(lonStr))
                {
                    errors.Add($"Hotel={HotelKey(hotel)} | Missing latitude/longitude (lat='{latStr ?? "null"}', lon='{lonStr ?? "null"}')");
                    continue;
                }

                if (!double.TryParse(latStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var lat) ||
                    !double.TryParse(lonStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var lon))
                {
                    errors.Add($"Hotel={HotelKey(hotel)} | Invalid coordinate format (lat='{latStr}', lon='{lonStr}')");
                    continue;
                }

                if (lat < -90 || lat > 90 || lon < -180 || lon > 180)
                {
                    errors.Add($"Hotel={HotelKey(hotel)} | Out-of-range coordinates (lat={lat}, lon={lon})");
                }
            }

            if (errors.Any())
            {
                LogFailures("Coordinates must be valid numeric lat/lon and within world range", errors);
                return false;
            }

            return true;
        }

        // NOTE: This checks UAE approximate bounds for EVERY hotel in the response.
        // Keep using it only for UAE countryCode, otherwise it will correctly report "violations".
        public bool UaeCoordinatesApproximate()
        {
            if (_response == null)
            {
                LogFailures("UAE coordinate bounds", new[] { "Response is null. Did the call succeed and parse?" });
                return false;
            }

            var errors = new List<string>();

            foreach (var hotel in _response)
            {
                var latStr = hotel?["latitude"]?.ToString();
                var lonStr = hotel?["longitude"]?.ToString();

                if (string.IsNullOrWhiteSpace(latStr) || string.IsNullOrWhiteSpace(lonStr))
                {
                    errors.Add($"Hotel={HotelKey(hotel)} | Missing latitude/longitude (lat='{latStr ?? "null"}', lon='{lonStr ?? "null"}')");
                    continue;
                }

                if (!double.TryParse(latStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var lat) ||
                    !double.TryParse(lonStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var lon))
                {
                    errors.Add($"Hotel={HotelKey(hotel)} | Invalid coordinate format (lat='{latStr}', lon='{lonStr}')");
                    continue;
                }

                if (lat < 22 || lat > 27 || lon < 51 || lon > 57)
                {
                    errors.Add($"Hotel={HotelKey(hotel)} | UAE approx bounds violated (lat={lat}, lon={lon})");
                }
            }

            if (errors.Any())
            {
                LogFailures("Coordinates must be inside UAE approximate bounds (lat 22..27, lon 51..57)", errors);
                return false;
            }

            return true;
        }

        private string HotelKey(JToken? hotel)
        {
            // No index: use business identifiers so PO/dev can find record quickly.
            var hotelId = hotel?["hotelID"]?.ToString();
            var hotelCode = hotel?["hotelCode"]?.ToString();
            var hotelName = hotel?["hotelName"]?.ToString();
            var cityUid = hotel?["cityUID"]?.ToString();

            // Keep it compact but searchable.
            return $"hotelID={hotelId ?? "?"}, hotelCode={hotelCode ?? "?"}, hotelName='{hotelName ?? "?"}', cityUID={cityUid ?? "?"}";
        }

        private void LogFailures(string rule, IEnumerable<string> errors)
        {
            TestContext.WriteLine("========================================");
            TestContext.WriteLine("[FAIL][GetHotels]");
            TestContext.WriteLine($"countryCode : {_currentCountryCode}");
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
    }
}
