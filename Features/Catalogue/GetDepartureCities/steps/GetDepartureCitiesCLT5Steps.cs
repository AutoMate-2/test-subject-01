using System.Threading.Tasks;
using NUnit.Framework;
using TechTalk.SpecFlow;
using to_integrations.CRUD.Catalogue.Api;
using to_integrations.HelperMethods;

namespace to_integrations.Features.Catalogue.GetDepartureCities.Steps
{
    [Binding]
    public class GetDepartureCitiesCLT5Steps
    {
        private readonly GetDepartureCitiesCrud _crud;
        private readonly ScenarioContext _scenarioContext;

        public GetDepartureCitiesCLT5Steps(GetDepartureCitiesCrud crud, ScenarioContext scenarioContext)
        {
            _crud = crud;
            _scenarioContext = scenarioContext;
        }

        [Then(@"each cityUID should be a valid UUID format for country ""(.*)""")]
        public async Task ThenEachCityUidShouldBeValidUuidFormatForCountry(string countryCode)
        {
            if (_crud.LastStatus == 0)
            {
                await _crud.CallGetDepartureCities(countryCode);
            }
            Assert.That(_crud.EachCityUidIsValidGuid(countryCode), Is.True,
                $"One or more cityUID values are not valid UUID format for {countryCode}");
        }

        [Then(@"each timeZoneOffset should be a number between -12 and 14 for country ""(.*)""")]
        public async Task ThenEachTimeZoneOffsetShouldBeNumberBetweenMinus12And14ForCountry(string countryCode)
        {
            if (_crud.LastStatus == 0)
            {
                await _crud.CallGetDepartureCities(countryCode);
            }
            Assert.That(_crud.EachTimeZoneOffsetValid(countryCode), Is.True,
                $"One or more timeZoneOffset values are not within the valid range (-12 to 14) for {countryCode}");
        }
    }
}
