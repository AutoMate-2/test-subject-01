using System.Threading.Tasks;
using NUnit.Framework;
using TechTalk.SpecFlow;
using to_integrations.CRUD.Catalogue.Api;
using to_integrations.HelperMethods;

namespace to_integrations.Features.Catalogue.GetDepartureCities.Steps
{
    [Binding]
    public class GetDepartureCitiesCLT4Steps
    {
        private readonly GetDepartureCitiesCrud _crud;
        private readonly ScenarioContext _scenarioContext;

        public GetDepartureCitiesCLT4Steps(GetDepartureCitiesCrud crud, ScenarioContext scenarioContext)
        {
            _crud = crud;
            _scenarioContext = scenarioContext;
        }

        [Then(@"each cityUID should be a valid UUID format for KZ")]
        public async Task ThenEachCityUidShouldBeValidUuidFormatForKZ()
        {
            if (_crud.LastStatus == 0)
            {
                await _crud.CallGetDepartureCities("KZ");
            }
            Assert.That(_crud.EachCityUidIsValidGuid("KZ"), Is.True, "One or more cityUID values are not valid UUID format for KZ");
        }

        [Then(@"each timeZoneOffset should be a number between -12 and 14 for KZ")]
        public async Task ThenEachTimeZoneOffsetShouldBeNumberBetweenMinus12And14ForKZ()
        {
            if (_crud.LastStatus == 0)
            {
                await _crud.CallGetDepartureCities("KZ");
            }
            Assert.That(_crud.EachTimeZoneOffsetValid("KZ"), Is.True, "One or more timeZoneOffset values are not within the valid range (-12 to 14) for KZ");
        }
    }
}
