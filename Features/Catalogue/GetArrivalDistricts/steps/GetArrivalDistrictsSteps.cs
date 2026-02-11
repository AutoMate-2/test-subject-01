using NUnit.Framework;
using TechTalk.SpecFlow;
using to_integrations.CRUD.Catalogue;
using to_integrations.HelperMethods;
using to_integrations.Presetup;

namespace to_integrations.Features.Catalogue.GetArrivalDistricts.Steps
{
    [Binding]
    public class GetArrivalDistrictsSteps
    {
        private readonly GetArrivalDistrictsCrud _crud;

        public GetArrivalDistrictsSteps(GetArrivalDistrictsCrud crud)
        {
            _crud = crud;
        }

        /* =========================================================
           SHARED HELPER — CHECK ALL COUNTRIES, FAIL ONCE
           ========================================================= */

        private async Task ForEachCountryAsync(Func<string, bool> assertion)
        {
            var failedCountries = new List<string>();

            foreach (var code in CountryCodesPresetup.All)
            {
                await _crud.CallAsync(code, TokenCache.CachedToken);

                if (_crud.LastStatus != 200)
                {
                    _crud.LogFailures(
                        "Status code must be 200",
                        new[] { $"Expected 200 but got {_crud.LastStatus}" },
                        code
                    );

                    failedCountries.Add($"{code} (status {_crud.LastStatus})");
                    continue;
                }

                if (!_crud.IsArray())
                {
                    _crud.LogFailures(
                        "Response must be array",
                        new[] { "Response is null or not a JSON array" },
                        code
                    );

                    failedCountries.Add($"{code} (not array)");
                    continue;
                }

                var ok = assertion(code);
                if (!ok)
                    failedCountries.Add(code);
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
           TC-TO-CAT-019
           ========================================================= */

        [Then(@"arrival districts response should be valid for all configured country codes")]
        public async Task ThenArrivalDistrictsResponseShouldBeValidForAllCountries()
        {
            await ForEachCountryAsync(_ =>
                _crud.IsArray()
            );
        }

        /* =========================================================
           TC-TO-CAT-020
           ========================================================= */

        [Then(@"arrival districts structure should be valid for all configured country codes")]
        public async Task ThenArrivalDistrictsStructureShouldBeValidForAllCountries()
        {
            await ForEachCountryAsync(code =>
                _crud.EachItemHasFields(
                    new[] { "type", "parent", "label", "value", "valueInt" },
                    code
                )
                && _crud.ContainsType("city", code)
                && _crud.ContainsType("district", code)
            );
        }

        /* =========================================================
           TC-TO-CAT-021
           ========================================================= */

        [Then(@"each district parent should reference a valid city for all configured country codes")]
        public async Task ThenDistrictParentShouldReferenceCityForAllCountries()
        {
            await ForEachCountryAsync(code =>
                _crud.DistrictParentsReferenceCities(code)
            );
        }

        /* =========================================================
           UAE-SPECIFIC
           ========================================================= */

        [Then(@"the arrival districts response should contain city ""(.*)""")]
        public void ThenArrivalDistrictsResponseShouldContainCity(string city)
        {
            Assert.That(
                _crud.ContainsCity(city),
                Is.True,
                $"City '{city}' not found in arrival districts response"
            );
        }
    }
}
