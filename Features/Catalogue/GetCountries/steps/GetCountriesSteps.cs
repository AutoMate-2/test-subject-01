using System.Threading.Tasks;
using NUnit.Framework;
using TechTalk.SpecFlow;
using to_integrations.CRUD.Catalogue.Api;
using to_integrations.HelperMethods;

namespace to_integrations.Features.Catalogue.GetCountries.Steps
{
    [Binding]
    public class GetCountriesSteps
    {
        private readonly GetCountriesCrud _crud;
        private readonly ScenarioContext _scenarioContext;

        public GetCountriesSteps(GetCountriesCrud crud, ScenarioContext scenarioContext)
        {
            _crud = crud;
            _scenarioContext = scenarioContext;
        }

        [Then(@"the response should contain ""countries"" array")]
        public void ThenResponseShouldContainCountriesArray()
        {
            Assert.That(_crud.HasCountriesArray(), Is.True, "Response does not contain 'countries' array");
        }

        [Then(@"the response should contain country with code ""(.*)""$")]
        public void ThenResponseShouldContainCountryCode(string code)
        {
            Assert.That(_crud.HasCountryCode(code), Is.True, $"Country with code '{code}' not found in response");
        }

        [Then(@"each ""phoneCode"" should be a valid international dialing code")]
        public void ThenPhoneCodesShouldBeValid()
        {
            Assert.That(_crud.ArePhoneCodesValid(), Is.True, "One or more phone codes are not valid international dialing codes");
        }

        [Then(@"each ""phoneCode"" should be numeric string")]
        public void ThenPhoneCodesShouldBeNumericString()
        {
            Assert.That(_crud.ArePhoneCodesNumericString(), Is.True, "One or more phone codes are not numeric strings");
        }
    }
}
