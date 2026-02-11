using NUnit.Framework;
using TechTalk.SpecFlow;
using System.Collections;
using to_integrations.CRUD.Auth;

namespace to_integrations.HelperMethods
{
    [Binding]
    public class CommonResponseSteps
    {
        private readonly AuthCrud _auth;
        private readonly ScenarioContext _scenarioContext;

        public CommonResponseSteps(AuthCrud auth, ScenarioContext scenarioContext)
        {
            _auth = auth;
            _scenarioContext = scenarioContext;
        }

        private dynamic ActiveCrud()
        {
            if (!string.IsNullOrWhiteSpace(_auth.LastBody))
                return _auth;

            throw new System.Exception("No API response found in any CRUD. Did you call a When-step before asserting?");
        }

        [Then("response code should be \"(.*)\"")]
        public void ThenCode(string code)
            => ActiveCrud().AssertCodeIs(code);

        [Then("response status should be \"(.*)\"")]
        public void ThenStatus(string status)
            => ActiveCrud().AssertCodeIs(status);

        [Then("response time should be under (\\d+) ms")]
        public void ThenResponseTimeUnder(int ms)
        {
            TestContext.WriteLine($"API Response Time: {LastResponse.TimeMs} ms");
            
            if (LastResponse.TimeMs >= ms)
            {
                var report = PlainTextReporter.CreateReport(_scenarioContext.ScenarioInfo.Title);
                report.Assertion = "Response time validation";
                report.Expected = $"Response time should be under {ms} ms";
                report.Actual = $"Response time is {LastResponse.TimeMs} ms";

                var step = PlainTextReporter.CreateStep("Then response time should be under " + ms + " ms");
                step.ErrorMessage = $"API too slow: {LastResponse.TimeMs} ms (limit {ms} ms)";
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }
        }

        [Then("error message for \"(.*)\" should contain \"(.*)\"")]
        public void ThenValidation(string field, string text)
        {
            ActiveCrud().AssertErrorContains(field, text);
        }

        [Then("business error message should contain \"(.*)\"")]
        public void ThenBusinessError(string text)
        {
            ActiveCrud().AssertBusinessMessageContains(text);
        }

        [Then("the response status code should be (.*)")]
        public void ThenTheResponseStatusCodeShouldBe(int statusCode)
        {
            var lastStatus = _scenarioContext.Get<int>("LastStatus");
            
            if (lastStatus != statusCode)
            {
                var report = PlainTextReporter.CreateReport(_scenarioContext.ScenarioInfo.Title);
                report.Assertion = "Response status code validation";
                report.Expected = $"Status code should be {statusCode}";
                report.Actual = $"Status code is {lastStatus}";

                var step = PlainTextReporter.CreateStep("Then the response status code should be " + statusCode);
                step.ErrorMessage = $"Expected status {statusCode} but got {lastStatus}";
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }
        }
    }
}

