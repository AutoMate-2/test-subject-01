using NUnit.Framework;
using TechTalk.SpecFlow;
using to_integrations.CRUD.Catalogue;
using to_integrations.HelperMethods;
using to_integrations.Presetup;

namespace to_integrations.Features.Catalogue.GetHotels.Steps
{
    [Binding]
    public class GetHotelsSteps
    {
        private readonly GetHotelsCrud _crud;

        public GetHotelsSteps(GetHotelsCrud crud)
        {
            _crud = crud;
        }

        /* =========================================================
           SHARED HELPER
           ========================================================= */

        private async Task ForEachCountryAsync(Func<string, bool> assertion)
        {
            var failedCountries = new List<string>();

            foreach (var countryCode in CountryCodesPresetup.All)
            {
                await _crud.CallAsync(countryCode, TokenCache.CachedToken);

                if (_crud.LastStatus != 200)
                {
                    failedCountries.Add($"{countryCode} (status {_crud.LastStatus})");
                    continue;
                }

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
           TC-TO-CAT-024
           ========================================================= */

        [Then(@"GetHotels response should be an array")]
        public async Task ThenResponseIsArrayForAllCountries()
        {
            foreach (var countryCode in CountryCodesPresetup.All)
            {
                await _crud.CallAsync(countryCode, TokenCache.CachedToken);

                Assert.That(
                    _crud.LastStatus,
                    Is.EqualTo(200),
                    $"Expected 200 for countryCode={countryCode}"
                );

                Assert.That(
                    _crud.IsArray(),
                    Is.True,
                    $"Response is not array for countryCode={countryCode}"
                );
            }
        }

        [Then(@"each hotel should have required fields:")]
        public async Task ThenEachHotelHasFieldsForAllCountries(Table table)
        {
            var fields = table.Rows.Select(r => r["Field"]).ToArray();

            await ForEachCountryAsync(_ =>
                _crud.EachHotelHasFields(fields)
            );
        }

        /* =========================================================
           TC-TO-CAT-026
           ========================================================= */

        [Then(@"each ""hotelCode"" should be a valid UUID")]
        public async Task ThenHotelCodeIsGuidForAllCountries()
        {
            await ForEachCountryAsync(_ =>
                _crud.EachHotelCodeIsGuid()
            );
        }

        [Then(@"each ""hotelClass"" should be one of: ""1"", ""2"", ""3"", ""4"", ""5"", ""6""")]
        public async Task ThenHotelClassValidForAllCountries()
        {
            await ForEachCountryAsync(_ =>
                _crud.EachHotelClassValid()
            );
        }

        [Then(@"each ""hotelType"" should be one of: ""CityHotel"", ""BeachHotel"", ""SecondLineBeach""")]
        public async Task ThenHotelTypeValidForAllCountries()
        {
            await ForEachCountryAsync(_ =>
                _crud.EachHotelTypeValid()
            );
        }

        /* =========================================================
           TC-TO-CAT-027
           ========================================================= */

        [Then(@"each ""(.*)"" should be a boolean")]
        public async Task ThenBooleanFieldValidForAllCountries(string field)
        {
            await ForEachCountryAsync(_ =>
                _crud.EachBooleanField(field)
            );
        }

        /* =========================================================
           TC-TO-CAT-028
           ========================================================= */

        [Then(@"each ""latitude"" should be a valid latitude \(-90 to 90\)")]
        [Then(@"each ""longitude"" should be a valid longitude \(-180 to 180\)")]
        public async Task ThenCoordinatesValidForAllCountries()
        {
            await ForEachCountryAsync(_ =>
                _crud.CoordinatesValid()
            );
        }

        /* =========================================================
           UAE-SPECIFIC ONLY
           ========================================================= */

        [Then(@"UAE hotels should have latitude approximately between 22 and 27")]
        [Then(@"UAE hotels should have longitude approximately between 51 and 57")]
        public async Task ThenUaeCoordinatesApproximate()
        {
            await _crud.CallAsync("AE", TokenCache.CachedToken);

            Assert.That(_crud.LastStatus, Is.EqualTo(200));

            if (_crud.IsEmptyArray())
                Assert.Inconclusive("No hotels returned for AE");

            Assert.That(
                _crud.UaeCoordinatesApproximate(),
                Is.True,
                "UAE hotel coordinates out of expected range"
            );
        }
    }
}
