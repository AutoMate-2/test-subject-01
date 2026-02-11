using NUnit.Framework;
using TechTalk.SpecFlow;
using to_integrations.CRUD.Catalogue;
using to_integrations.HelperMethods;
using to_integrations.Presetup;

namespace to_integrations.Features.Catalogue.GetFlightDateOptions.Steps
{
    [Binding]
    public class GetFlightDateOptionsSteps
    {
        private readonly GetFlightDateOptionsCrud _crud;
        private readonly ScenarioContext _scenarioContext;

        private int _requestedMonth;
        private int _requestedYear;

        public GetFlightDateOptionsSteps(
            GetFlightDateOptionsCrud crud,
            ScenarioContext scenarioContext)
        {
            _crud = crud;
            _scenarioContext = scenarioContext;
        }

        private static (int Month, int Year) FutureMonth()
        {
            var dt = DateTime.UtcNow.AddMonths(1);
            return (dt.Month, dt.Year);
        }

        private static (int Month, int Year) PastMonth()
        {
            var dt = DateTime.UtcNow.AddYears(-1);
            return (dt.Month, dt.Year);
        }

        private async Task ForEachCountryCollectingAsync(
            Func<string, Task<bool>> perCountryCheck,
            string scenarioRule)
        {
            var failed = new List<string>();

            foreach (var countryCode in CountryCodesPresetup.All)
            {
                TestContext.WriteLine($"[RUN][FlightDateOptions] countryCode={countryCode}");

                var okPresetup =
                    await FlightDateOptionsCityPresetup.TryResolveDepartureCityAsync(countryCode);

                if (!okPresetup)
                    continue;

                var ok = await perCountryCheck(countryCode);
                if (!ok)
                    failed.Add(countryCode);
            }

            if (failed.Any())
            {
                Assert.Fail(
                    $"[GetFlightDateOptions] Failures for rule: {scenarioRule}\n" +
                    string.Join(", ", failed)
                );
            }
        }

        /* =============================
           WHEN
           ============================= */

        [Then(@"I request flight date options for a future month")]
        public async Task WhenIRequestFlightDateOptionsForFutureMonth()
        {
            var (month, year) = FutureMonth();
            _requestedMonth = month;
            _requestedYear = year;

            await ForEachCountryCollectingAsync(async countryCode =>
            {
                var cityUid = FlightDateOptionsCityPresetup.GetCityUid(countryCode);

                await _crud.CallAsync(
                    cityUid,
                    countryCode,
                    true,
                    month,
                    year,
                    TokenCache.CachedToken
                );

                if (_crud.LastStatus != 200)
                {
                    TestContext.WriteLine(
                        $"[FAIL][GetFlightDateOptions] countryCode={countryCode} status={_crud.LastStatus}"
                    );
                    return false;
                }

                return true;
            }, "Future month request should return 200");

            _scenarioContext.Set(200, "LastStatus");
        }

        [Then(@"I request flight date options for a specific month")]
        public async Task WhenIRequestFlightDateOptionsForSpecificMonth()
        {
            var (month, year) = FutureMonth();
            _requestedMonth = month;
            _requestedYear = year;

            await ForEachCountryCollectingAsync(async countryCode =>
            {
                var cityUid = FlightDateOptionsCityPresetup.GetCityUid(countryCode);

                await _crud.CallAsync(
                    cityUid,
                    countryCode,
                    true,
                    month,
                    year,
                    TokenCache.CachedToken
                );

                if (_crud.LastStatus != 200)
                {
                    TestContext.WriteLine(
                        $"[FAIL][GetFlightDateOptions] countryCode={countryCode} status={_crud.LastStatus}"
                    );
                    return false;
                }

                return true;
            }, "Specific month request should return 200");

            _scenarioContext.Set(200, "LastStatus");
        }

        [Then(@"I request flight date options for a past month")]
        public async Task WhenIRequestFlightDateOptionsForPastMonth()
        {
            var (month, year) = PastMonth();
            _requestedMonth = month;
            _requestedYear = year;

            await ForEachCountryCollectingAsync(async countryCode =>
            {
                var cityUid = FlightDateOptionsCityPresetup.GetCityUid(countryCode);

                await _crud.CallAsync(
                    cityUid,
                    countryCode,
                    true,
                    month,
                    year,
                    TokenCache.CachedToken
                );

                if (_crud.LastStatus != 200)
                {
                    TestContext.WriteLine(
                        $"[FAIL][GetFlightDateOptions] countryCode={countryCode} status={_crud.LastStatus}"
                    );
                    return false;
                }

                return true;
            }, "Past month request should return 200");

            _scenarioContext.Set(200, "LastStatus");
        }

        [When(@"I request flight date options with an invalid cityUID")]
        public async Task WhenIRequestWithInvalidCityUid()
        {
            var (month, year) = FutureMonth();
            _requestedMonth = month;
            _requestedYear = year;

            var failed = new List<string>();

            foreach (var countryCode in CountryCodesPresetup.All)
            {
                await _crud.CallAsync(
                    "invalid-uuid",
                    countryCode,
                    true,
                    month,
                    year,
                    TokenCache.CachedToken
                );

                if (_crud.LastStatus != 400)
                {
                    TestContext.WriteLine(
                        $"[FAIL][GetFlightDateOptions] countryCode={countryCode} expected 400 but got {_crud.LastStatus}"
                    );
                    failed.Add(countryCode);
                }
            }

            if (failed.Any())
            {
                Assert.Fail(
                    "[GetFlightDateOptions] Invalid CityUID should return 400. Failed: " +
                    string.Join(", ", failed)
                );
            }

            _scenarioContext.Set(400, "LastStatus");
        }

        /* =============================
           THEN
           ============================= */

        [Then(@"the response should contain ""availableDates"" array")]
        public void ThenHasAvailableDates()
            => Assert.That(_crud.HasAvailableDatesArray(), Is.True);

        [Then(@"each date should have:")]
        public void ThenEachDateHasFields(Table table)
        {
            var fields = table.Rows.Select(r => r["Field"]);
            Assert.That(_crud.EachDateHasFields(fields), Is.True);
        }

        [Then(@"the response for flightoptions should contain ""remainingTicketLimit""")]
        public void ThenHasRemainingTicketLimit()
            => Assert.That(_crud.HasRemainingTicketLimit(), Is.True);

        [Then(@"""remainingTicketLimit"" should be a positive integer")]
        public void ThenRemainingTicketLimitPositive()
            => Assert.That(_crud.RemainingTicketLimitPositive(), Is.True);

        [Then(@"all ""departureDate"" values should belong to the requested month and year")]
        public void ThenDatesWithinRequestedMonth()
            => Assert.That(_crud.DatesWithinMonth(_requestedMonth, _requestedYear), Is.True);

        [Then(@"the ""availableDates"" array should be empty")]
        public void ThenAvailableDatesEmpty()
            => Assert.That(_crud.AvailableDatesEmpty(), Is.True);
    }
}
