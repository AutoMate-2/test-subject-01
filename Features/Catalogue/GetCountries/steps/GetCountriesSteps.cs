using NUnit.Framework;
using TechTalk.SpecFlow;
using to_integrations.CRUD.Catalogue;
using to_integrations.CRUD.Catalogue.Api;
using to_integrations.HelperMethods;

namespace to_integrations.Features.Catalogue.GetCountries.Steps
{
    [Binding]
    public class GetCountriesSteps
    {
        private readonly GetCountriesCrud _crud;

        public GetCountriesSteps(GetCountriesCrud crud)
        {
            _crud = crud;
        }

        [Then(@"the response should contain country with code ""(.*)""")]
        public void ThenResponseShouldContainCountryCode(string code)
        {
            Assert.That(_crud.HasCountryCode(code), Is.True,
                $"Country with code '{code}' not found");
        }

        [Then(@"each ""(.*)"" should be a valid international dialing code")]
        public void ThenPhoneCodesShouldBeValid(string field)
        {
            Assert.That(_crud.ArePhoneCodesValid(), Is.True,
                "One or more phone codes are invalid");
        }

        [Then(@"the response should contain ""countries"" array")]
        public void ThenResponseShouldContainCountriesArray()
        {
            Assert.That(_crud.HasCountriesArray(), Is.True,
                "Response does not contain 'countries' array");
        }

    }
}
