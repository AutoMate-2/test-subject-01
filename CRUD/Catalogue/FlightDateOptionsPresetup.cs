using Newtonsoft.Json.Linq;
using NUnit.Framework;
using to_integrations.CRUD.Catalogue;
using to_integrations.HelperMethods;

namespace to_integrations.Presetup
{
    public static class FlightDateOptionsCityPresetup
    {
        // countryCode -> cityUID
        private static readonly Dictionary<string, string> _departureCityByCountry
            = new(StringComparer.OrdinalIgnoreCase);

        public static bool HasCity(string countryCode)
            => _departureCityByCountry.ContainsKey(countryCode);

        public static string GetCityUid(string countryCode)
        {
            Assert.That(
                _departureCityByCountry.ContainsKey(countryCode),
                Is.True,
                $"Departure cityUID was not presetup for countryCode={countryCode}"
            );

            return _departureCityByCountry[countryCode];
        }

        /// <summary>
        /// Tries to resolve a departure cityUID for the given countryCode.
        /// NEVER asserts. Returns false when data is missing (so tests can continue for other countries).
        /// </summary>
        public static async Task<bool> TryResolveDepartureCityAsync(string countryCode)
        {
            if (_departureCityByCountry.ContainsKey(countryCode))
                return true;

            var crud = new GetDepartureCitiesCrud();

            await crud.CallGetDepartureCities(countryCode, TokenCache.CachedToken);

            if (crud.LastStatus != 200)
            {
                TestContext.WriteLine(
                    $"[PRESETUP][FlightDateOptions][FAIL] GetDepartureCities status {crud.LastStatus} for countryCode={countryCode}"
                );
                return false;
            }

            if (crud.IsEmptyArray())
            {
                TestContext.WriteLine(
                    $"[PRESETUP][FlightDateOptions][SKIP] No departure cities returned for countryCode={countryCode}"
                );
                return false;
            }

            // Pick FIRST valid city (business-agnostic, stable)
            var citiesField = typeof(GetDepartureCitiesCrud)
                .GetField("_cities",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);

            var cities = citiesField?.GetValue(crud) as JArray;

            var firstCity = cities?
                .FirstOrDefault(c => Guid.TryParse(c["cityUID"]?.ToString(), out _));

            if (firstCity == null)
            {
                TestContext.WriteLine(
                    $"[PRESETUP][FlightDateOptions][SKIP] No valid cityUID found for countryCode={countryCode}"
                );
                return false;
            }

            var cityUid = firstCity["cityUID"]!.ToString();
            _departureCityByCountry[countryCode] = cityUid;

            TestContext.WriteLine(
                $"[PRESETUP][FlightDateOptions] countryCode={countryCode} -> cityUID={cityUid}"
            );

            return true;
        }

        // Keep old method name if other code already calls it:
        // It now delegates to TryResolve and DOES NOT assert.
        public static async Task ResolveDepartureCityAsync(string countryCode)
            => await TryResolveDepartureCityAsync(countryCode);
    }
}
