using TechTalk.SpecFlow;
using to_integrations.CRUD.Auth;

namespace to_integrations.Features.Auth.Steps
{
    [Binding]
    public class AuthSteps
    {
        private readonly AuthCrud _crud;
        private readonly ScenarioContext _scenarioContext;
        private string _login = "";
        private string _password = "";

        public AuthSteps(AuthCrud crud, ScenarioContext scenarioContext)
        {
            _crud = crud;
            _scenarioContext = scenarioContext;
        }

        private void SetTestCaseFromScenario()
        {
            var scenarioTitle = _scenarioContext.ScenarioInfo.Title;
            _crud.SetTestCase(scenarioTitle);
        }

        [Given("I have valid login credentials")]
        public void GivenIHaveValidLoginCredentials()
        {
            SetTestCaseFromScenario();
            _login = AuthPresetup.ValidLogin;
            _password = AuthPresetup.ValidPassword;
        }

        [Given("I have invalid login credentials")]
        public void GivenIHaveInvalidLoginCredentials()
        {
            SetTestCaseFromScenario();
            _login = AuthPresetup.InvalidLogin;
            _password = AuthPresetup.InvalidPassword;
        }

        [Given("I have login credentials with empty login")]
        public void GivenIHaveLoginCredentialsWithEmptyLogin()
        {
            SetTestCaseFromScenario();
            _login = "";
            _password = AuthPresetup.ValidPassword;
        }

        [Given("I have login credentials with empty password")]
        public void GivenIHaveLoginCredentialsWithEmptyPassword()
        {
            SetTestCaseFromScenario();
            _login = AuthPresetup.ValidLogin;
            _password = "";
        }

        [Given("I have no request body")]
        public void GivenIHaveNoRequestBody()
        {
            SetTestCaseFromScenario();
        }

        [Given("I have login credentials with single character login")]
        public void GivenIHaveLoginCredentialsWithSingleCharLogin()
        {
            SetTestCaseFromScenario();
            _login = AuthPresetup.SingleCharLogin;
            _password = AuthPresetup.ValidPassword;
        }

        [Given("I have login credentials with 255 character login")]
        public void GivenIHaveLoginCredentialsWith255CharLogin()
        {
            SetTestCaseFromScenario();
            _login = AuthPresetup.LongString255;
            _password = AuthPresetup.ValidPassword;
        }

        [Given("I have login credentials with single character password")]
        public void GivenIHaveLoginCredentialsWithSingleCharPassword()
        {
            SetTestCaseFromScenario();
            _login = AuthPresetup.ValidLogin;
            _password = AuthPresetup.SingleCharPassword;
        }

        [Given("I have login credentials with 255 character password")]
        public void GivenIHaveLoginCredentialsWith255CharPassword()
        {
            SetTestCaseFromScenario();
            _login = AuthPresetup.ValidLogin;
            _password = AuthPresetup.LongString255;
        }

        [Given("I have login credentials with SQL injection")]
        public void GivenIHaveLoginCredentialsWithSqlInjection()
        {
            SetTestCaseFromScenario();
            _login = AuthPresetup.SqlInjectionString;
            _password = AuthPresetup.ValidPassword;
        }

        [Given("I have login credentials with XSS script")]
        public void GivenIHaveLoginCredentialsWithXssScript()
        {
            SetTestCaseFromScenario();
            _login = AuthPresetup.XssScriptString;
            _password = AuthPresetup.ValidPassword;
        }

        [When("I send a POST request to Auth endpoint")]
        public async Task WhenISendAPostRequestToAuthEndpoint()
        {
            await _crud.LoginAsync(_login, _password);
            _scenarioContext.Set(_crud.LastStatus, "LastStatus");
        }

        [When("I send a POST request to Auth endpoint without body")]
        public async Task WhenISendAPostRequestToAuthEndpointWithoutBody()
        {
            await _crud.LoginWithoutBodyAsync();
            _scenarioContext.Set(_crud.LastStatus, "LastStatus");
        }

        [Then("the response should contain \"(.*)\"")]
        public void ThenTheResponseShouldContain(string fieldName)
        {
            _crud.AssertResponseContains(fieldName);
        }

        [Then("the response \"(.*)\" should be a boolean")]
        public void ThenTheResponseFieldShouldBeABoolean(string fieldName)
        {
            _crud.AssertFieldIsBoolean(fieldName);
        }

        [Then("the response should contain error message")]
        public void ThenTheResponseShouldContainErrorMessage()
        {
            _crud.AssertResponseContainsErrorMessage();
        }

        [Then("the response should contain validation error for \"(.*)\"")]
        public void ThenTheResponseShouldContainValidationErrorFor(string fieldName)
        {
            _crud.AssertValidationErrorForField(fieldName);
        }

        [Then("the \"(.*)\" should be a valid JWT token")]
        public void ThenTheFieldShouldBeAValidJwtToken(string fieldName)
        {
            _crud.AssertAccessTokenIsValidJwt();
        }

        [Then("the token expiration should be approximately (\\d+) minutes")]
        public void ThenTheTokenExpirationShouldBeApproximatelyMinutes(int minutes)
        {
            _crud.AssertTokenExpirationIsApproximately(minutes);
        }

        [Then("the JWT payload should not contain password")]
        public void ThenTheJwtPayloadShouldNotContainPassword()
        {
            _crud.AssertJwtPayloadDoesNotContainPassword();
        }

        [Then("the JWT payload should not contain sensitive user data")]
        public void ThenTheJwtPayloadShouldNotContainSensitiveUserData()
        {
            _crud.AssertJwtPayloadDoesNotContainSensitiveData();
        }
    }
}
