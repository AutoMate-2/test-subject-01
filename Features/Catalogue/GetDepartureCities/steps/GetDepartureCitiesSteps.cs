using NUnit.Framework;
using TechTalk.SpecFlow;
using to_integrations.CRUD.Catalogue;
using to_integrations.HelperMethods;
using to_integrations.Presetup;

namespace to_integrations.Features.Catalogue.GetDepartureCities.Steps
{
    [Binding]
    public class GetDepartureCitiesSteps
    {
        private readonly GetDepartureCitiesCrud _crud;

        public GetDepartureCitiesSteps(GetDepartureCitiesCrud crud)
        {
            _crud = crud;
        }

        /* =========================================================
           SHARED HELPER (do NOT stop on first failure)
           ========================================================= */

        private async Task ForEachCountryAsync(Func<string, bool> assertion)
        {
            var failedCountries = new List<string>();

            foreach (var countryCode in CountryCodesPresetup.All)
            {
                await _crud.CallGetDepartureCities(countryCode, TokenCache.CachedToken);

                // Status must be 200 to validate the response shape/content.
                if (_crud.LastStatus != 200)
                {
                    _crud.LogFailures("Status code must be 200", new[]
                    {
                        $"Expected 200 but got {_crud.LastStatus}"
                    }, countryCode);

                    failedCountries.Add($"{countryCode} (status {_crud.LastStatus})");
                    continue;
                }

                // If parse failed, _cities == null
                if (!_crud.IsArray())
                {
                    _crud.LogFailures("Response must be a JSON array", new[]
                    {
                        "Response is not a valid JSON array (parse failed or null)"
                    }, countryCode);

                    failedCountries.Add($"{countryCode} (not array)");
                    continue;
                }

                // Empty is allowed; skip further content validations.
                if (_crud.IsEmptyArray())
                    continue;

                var ok = assertion(countryCode);
                if (!ok)
                    failedCountries.Add(countryCode);
            }

            if (failedCountries.Any())
            {
                Assert.Fail(
                    "Failures detected for country codes:\n" +
                    string.Join(", ", failedCountries)
                );
            }
        }

        /* =========================================================
           Feature steps (kept same signatures)
           ========================================================= */

        [Then(@"departure cities response should be valid for all configured country codes")]
        public async Task ThenDepartureCitiesResponseShouldBeValidForAllConfiguredCountryCodes()
        {
            await ForEachCountryAsync(countryCode =>
                // This step only requires "array" (as your original)
                _crud.IsArray()
            );
        }

        [Then(@"each cityUID should be valid for all configured country codes")]
        public async Task ThenEachCityUidShouldBeValidForAllConfiguredCountryCodes()
        {
            await ForEachCountryAsync(countryCode =>
                _crud.EachCityUidIsValidGuid(countryCode)
            );
        }

        [Then(@"each timeZoneOffset should be valid for all configured country codes")]
        public async Task ThenEachTimeZoneOffsetShouldBeValidForAllConfiguredCountryCodes()
        {
            await ForEachCountryAsync(countryCode =>
                _crud.EachTimeZoneOffsetValid(countryCode)
            );
        }

        // These generic steps remain unchanged (they validate the LAST response already loaded by some When step).
        [Then(@"the response should be an array")]
        public void ThenResponseShouldBeArray()
            => Assert.That(_crud.IsArray(), Is.True);

        [Then(@"the response should be an empty array")]
        public void ThenResponseShouldBeEmptyArray()
            => Assert.That(_crud.IsEmptyArray(), Is.True);

        [Then(@"the response should contain city ""(.*)""")]
        public void ThenResponseShouldContainCity(string city)
            => Assert.That(_crud.HasCity(city), Is.True);

        [Then(@"each city should have required fields:")]
        public void ThenEachCityShouldHaveFields(Table table)
        {
            var fields = table.Rows.Select(r => r["Field"]).ToArray();
            Assert.That(_crud.EachCityHasFields(fields), Is.True);
        }
    }
}
