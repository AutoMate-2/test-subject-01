using NUnit.Framework;
using TechTalk.SpecFlow;
using to_integrations.CRUD.Catalogue;

namespace to_integrations.HelperMethods
{
    [Binding]
    public class CommonAuthSteps
    {
        [Given(@"I am authenticated with valid token")]
        public void GivenIAmAuthenticatedWithValidToken()
        {
            Assert.That(
                CataloguePresetup.ValidToken,
                Is.Not.Empty,
                "Valid token was not initialized. Check BeforeTestRun hook."
            );
        }
    }
}
  