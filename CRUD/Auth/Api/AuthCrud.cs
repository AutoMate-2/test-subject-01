using Newtonsoft.Json.Linq;
using NUnit.Framework;
using to_integrations.HelperMethods;
using to_integrations.CRUD.Auth.Api;
using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace to_integrations.CRUD.Auth
{
    public class AuthCrud
    {
        private readonly AuthApi _api = new AuthApi();
        private JObject? _response;
        private string _lastUrl = "";
        private string _lastRequestBody = "";
        private string _currentTestCase = "";

        public int LastStatus { get; private set; }
        public string LastBody { get; private set; } = "";

        public async Task LoginWithValidCredentialsAsync()
        {
            await LoginAsync(AuthPresetup.ValidLogin, AuthPresetup.ValidPassword);
        }

        public async Task LoginAsync(string login, string password)
        {
            var sw = Stopwatch.StartNew();
            var (status, body) = await _api.LoginAsync(login, password);
            sw.Stop();

            LastResponse.TimeMs = sw.ElapsedMilliseconds;
            LastStatus = status;
            LastBody = body;
            _lastUrl = "POST https://integrations-main-nws.nugios.test/api/Auth/Login";
            _lastRequestBody = $"{{ \"login\": \"{login}\", \"password\": \"***\" }}";

            LogRequest(login);
            LogResponse(status, body);
            ParseResponse(body);
        }

        public async Task LoginWithoutBodyAsync()
        {
            var sw = Stopwatch.StartNew();
            var (status, body) = await _api.LoginWithoutBodyAsync();
            sw.Stop();

            LastResponse.TimeMs = sw.ElapsedMilliseconds;
            LastStatus = status;
            LastBody = body;
            _lastUrl = "POST https://integrations-main-nws.nugios.test/api/Auth/Login";
            _lastRequestBody = "(empty)";

            LogResponse(status, body);
            ParseResponse(body);
        }

        private void LogRequest(string login)
        {
            Console.WriteLine("====== AUTH REQUEST ======");
            Console.WriteLine($"URL: {_lastUrl}");
            Console.WriteLine($"Request Body: {{ \"login\": \"{login}\", \"password\": \"***\" }}");
            Console.WriteLine("==========================");
        }

        private void LogResponse(int status, string body)
        {
            Console.WriteLine("====== AUTH RESPONSE ======");
            Console.WriteLine($"Status Code: {status}");
            Console.WriteLine("Response Body:");
            
            try
            {
                var beautifiedJson = JToken.Parse(body).ToString(Newtonsoft.Json.Formatting.Indented);
                Console.WriteLine(beautifiedJson);
            }
            catch
            {
                Console.WriteLine(body);
            }
            
            Console.WriteLine("==========================");
        }

        private void ParseResponse(string body)
        {
            try
            {
                _response = JObject.Parse(body);
            }
            catch
            {
                _response = null;
            }
        }

        public void SetTestCase(string testCase)
        {
            _currentTestCase = testCase;
        }

        public void AssertStatusCodeIs(int expectedCode)
        {
            if (LastStatus != expectedCode)
            {
                var report = PlainTextReporter.CreateReport(_currentTestCase);
                report.Assertion = "Response status code validation";
                report.Expected = $"Status code should be {expectedCode}";
                report.Actual = $"Status code is {LastStatus}";

                var step = PlainTextReporter.CreateStep("When I send a POST request to Auth endpoint");
                step.Url = _lastUrl;
                step.ErrorMessage = $"Expected status {expectedCode} but got {LastStatus}";
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }
        }

        public void AssertCodeIs(string expected)
        {
            if (_response?["Code"] != null)
            {
                var actual = _response["Code"]!.ToString();
                if (actual != expected)
                {
                    var report = PlainTextReporter.CreateReport(_currentTestCase);
                    report.Assertion = "Response code validation";
                    report.Expected = $"Code should be {expected}";
                    report.Actual = $"Code is {actual}";

                    var step = PlainTextReporter.CreateStep("When I send a POST request to Auth endpoint");
                    step.Url = _lastUrl;
                    step.ErrorMessage = $"Expected code {expected} but got {actual}";
                    report.BddSteps.Add(step);

                    PlainTextReporter.ThrowFailure(report);
                }
                return;
            }

            if (LastStatus.ToString() != expected)
            {
                var report = PlainTextReporter.CreateReport(_currentTestCase);
                report.Assertion = "Response status validation";
                report.Expected = $"Status should be {expected}";
                report.Actual = $"Status is {LastStatus}";

                var step = PlainTextReporter.CreateStep("When I send a POST request to Auth endpoint");
                step.Url = _lastUrl;
                step.ErrorMessage = $"Expected status {expected} but got {LastStatus}";
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }
        }

        public void AssertResponseContains(string fieldName)
        {
            if (_response?[fieldName] == null)
            {
                var report = PlainTextReporter.CreateReport(_currentTestCase);
                report.Assertion = $"Response field '{fieldName}' presence";
                report.Expected = $"Response should contain field '{fieldName}'";
                report.Actual = $"Field '{fieldName}' is missing from response";

                var step = PlainTextReporter.CreateStep("When I send a POST request to Auth endpoint");
                step.Url = _lastUrl;
                step.ErrorMessage = $"Response does not contain '{fieldName}'";
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }
        }

        public void AssertFieldIsBoolean(string fieldName)
        {
            var value = _response?[fieldName];
            
            if (value == null)
            {
                var report = PlainTextReporter.CreateReport(_currentTestCase);
                report.Assertion = $"Field '{fieldName}' type validation";
                report.Expected = $"Field '{fieldName}' should be a boolean";
                report.Actual = $"Field '{fieldName}' not found in response";

                var step = PlainTextReporter.CreateStep("When I send a POST request to Auth endpoint");
                step.Url = _lastUrl;
                step.ErrorMessage = $"Field '{fieldName}' not found";
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }

            if (value!.Type != JTokenType.Boolean)
            {
                var report = PlainTextReporter.CreateReport(_currentTestCase);
                report.Assertion = $"Field '{fieldName}' type validation";
                report.Expected = $"Field '{fieldName}' should be a boolean";
                report.Actual = $"Field '{fieldName}' is type {value.Type}";

                var step = PlainTextReporter.CreateStep("When I send a POST request to Auth endpoint");
                step.Url = _lastUrl;
                step.ErrorMessage = $"Field '{fieldName}' is not a boolean";
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }
        }

        public void AssertResponseContainsErrorMessage()
        {
            var hasError = _response?["error"] != null ||
                           _response?["message"] != null ||
                           _response?["Message"] != null ||
                           _response?["errors"] != null;

            if (!hasError)
            {
                var report = PlainTextReporter.CreateReport(_currentTestCase);
                report.Assertion = "Error message presence";
                report.Expected = "Response should contain error message";
                report.Actual = "No error field found in response";

                var step = PlainTextReporter.CreateStep("When I send a POST request to Auth endpoint");
                step.Url = _lastUrl;
                step.ErrorMessage = "Response does not contain error message";
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }
        }

        public void AssertValidationErrorForField(string fieldName)
        {
            var errors = _response?["errors"];
            if (errors != null)
            {
                var fieldError = errors[fieldName] ?? errors[fieldName.ToLower()] ?? errors[char.ToUpper(fieldName[0]) + fieldName.Substring(1)];
                if (fieldError != null) return;
            }

            var message = _response?["message"]?.ToString() ?? _response?["Message"]?.ToString() ?? "";
            if (message.ToLower().Contains(fieldName.ToLower())) return;

            var report = PlainTextReporter.CreateReport(_currentTestCase);
            report.Assertion = $"Validation error for field '{fieldName}'";
            report.Expected = $"Response should contain validation error for '{fieldName}'";
            report.Actual = $"No validation error found for field '{fieldName}'";

            var step = PlainTextReporter.CreateStep("When I send a POST request to Auth endpoint");
            step.Url = _lastUrl;
            step.ErrorMessage = $"No validation error found for field '{fieldName}'";
            report.BddSteps.Add(step);

            PlainTextReporter.ThrowFailure(report);
        }

        public void AssertBusinessMessageContains(string text)
        {
            var msg = _response?["message"]?.ToString() ??
                      _response?["Message"]?.ToString() ??
                      _response?["error"]?.ToString() ?? "";

            if (!msg.ToLower().Contains(text.ToLower()))
            {
                var report = PlainTextReporter.CreateReport(_currentTestCase);
                report.Assertion = "Business message content";
                report.Expected = $"Message should contain '{text}'";
                report.Actual = $"Message is '{msg}'";

                var step = PlainTextReporter.CreateStep("When I send a POST request to Auth endpoint");
                step.Url = _lastUrl;
                step.ErrorMessage = $"Expected message to contain '{text}' but was '{msg}'";
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }
        }

        public void AssertErrorContains(string field, string expectedText)
        {
            var errors = _response?["errors"]?[field] as JArray;

            if (errors != null)
            {
                var combined = string.Join(" ", errors.Select(x => x.ToString()));
                if (combined.ToLower().Contains(expectedText.ToLower())) return;
            }

            var altErrors = _response?["errors"]?[char.ToUpper(field[0]) + field.Substring(1)] as JArray ??
                            _response?["errors"]?[field.ToLower()] as JArray;

            if (altErrors != null)
            {
                var altCombined = string.Join(" ", altErrors.Select(x => x.ToString()));
                if (altCombined.ToLower().Contains(expectedText.ToLower())) return;
            }

            var report = PlainTextReporter.CreateReport(_currentTestCase);
            report.Assertion = $"Validation error content for '{field}'";
            report.Expected = $"Error for '{field}' should contain '{expectedText}'";
            report.Actual = errors != null ? $"Error is '{string.Join(" ", errors.Select(x => x.ToString()))}'" : $"No errors found for field '{field}'";

            var step = PlainTextReporter.CreateStep("When I send a POST request to Auth endpoint");
            step.Url = _lastUrl;
            step.ErrorMessage = $"Validation error for '{field}' does not contain '{expectedText}'";
            report.BddSteps.Add(step);

            PlainTextReporter.ThrowFailure(report);
        }

        public void AssertAccessTokenIsValidJwt()
        {
            var token = _response?["accessToken"]?.ToString();
            
            if (string.IsNullOrEmpty(token))
            {
                var report = PlainTextReporter.CreateReport(_currentTestCase);
                report.Assertion = "JWT token validation";
                report.Expected = "accessToken should be a valid JWT token";
                report.Actual = "accessToken is null or empty";

                var step = PlainTextReporter.CreateStep("When I send a POST request to Auth endpoint");
                step.Url = _lastUrl;
                step.ErrorMessage = "accessToken is null or empty";
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                if (jwtToken == null)
                {
                    throw new Exception("Failed to parse JWT token");
                }
            }
            catch (Exception ex)
            {
                var report = PlainTextReporter.CreateReport(_currentTestCase);
                report.Assertion = "JWT token validation";
                report.Expected = "accessToken should be a valid JWT token";
                report.Actual = $"accessToken is not a valid JWT: {ex.Message}";

                var step = PlainTextReporter.CreateStep("When I send a POST request to Auth endpoint");
                step.Url = _lastUrl;
                step.ErrorMessage = $"accessToken is not a valid JWT: {ex.Message}";
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }
        }

        public void AssertTokenExpirationIsApproximately(int expectedMinutes, int toleranceMinutes = 60)
        {
            var token = _response?["accessToken"]?.ToString();
            
            if (string.IsNullOrEmpty(token))
            {
                var report = PlainTextReporter.CreateReport(_currentTestCase);
                report.Assertion = "Token expiration validation";
                report.Expected = $"Token should expire in approximately {expectedMinutes} minutes";
                report.Actual = "accessToken is null or empty";

                var step = PlainTextReporter.CreateStep("When I send a POST request to Auth endpoint");
                step.Url = _lastUrl;
                step.ErrorMessage = "accessToken is null or empty";
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var expiration = jwtToken.ValidTo;
            var now = DateTime.UtcNow;
            var actualMinutes = (expiration - now).TotalMinutes;

            if (actualMinutes < expectedMinutes - toleranceMinutes || actualMinutes > expectedMinutes + toleranceMinutes)
            {
                var report = PlainTextReporter.CreateReport(_currentTestCase);
                report.Assertion = "Token expiration validation";
                report.Expected = $"Token should expire in approximately {expectedMinutes} minutes";
                report.Actual = $"Token expires in {Math.Round(actualMinutes)} minutes";

                var step = PlainTextReporter.CreateStep("When I send a POST request to Auth endpoint");
                step.Url = _lastUrl;
                step.ErrorMessage = $"Token expiration is {Math.Round(actualMinutes)} minutes, expected {expectedMinutes}";
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }
        }

        public void AssertJwtPayloadDoesNotContainPassword()
        {
            var token = _response?["accessToken"]?.ToString();
            
            if (string.IsNullOrEmpty(token))
            {
                var report = PlainTextReporter.CreateReport(_currentTestCase);
                report.Assertion = "JWT payload password check";
                report.Expected = "JWT payload should not contain password";
                report.Actual = "accessToken is null or empty";

                var step = PlainTextReporter.CreateStep("When I send a POST request to Auth endpoint");
                step.Url = _lastUrl;
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var payload = jwtToken.Payload.SerializeToJson().ToLower();

            if (payload.Contains("password"))
            {
                var report = PlainTextReporter.CreateReport(_currentTestCase);
                report.Assertion = "JWT payload password check";
                report.Expected = "JWT payload should not contain 'password' field";
                report.Actual = "JWT payload contains 'password' field";

                var step = PlainTextReporter.CreateStep("When I send a POST request to Auth endpoint");
                step.Url = _lastUrl;
                step.ErrorMessage = "JWT payload contains 'password' field";
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }

            if (payload.Contains(AuthPresetup.ValidPassword.ToLower()))
            {
                var report = PlainTextReporter.CreateReport(_currentTestCase);
                report.Assertion = "JWT payload password check";
                report.Expected = "JWT payload should not contain actual password value";
                report.Actual = "JWT payload contains actual password value";

                var step = PlainTextReporter.CreateStep("When I send a POST request to Auth endpoint");
                step.Url = _lastUrl;
                step.ErrorMessage = "JWT payload contains actual password value";
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }
        }

        public void AssertJwtPayloadDoesNotContainSensitiveData()
        {
            var token = _response?["accessToken"]?.ToString();
            
            if (string.IsNullOrEmpty(token))
            {
                var report = PlainTextReporter.CreateReport(_currentTestCase);
                report.Assertion = "JWT payload sensitive data check";
                report.Expected = "JWT payload should not contain sensitive data";
                report.Actual = "accessToken is null or empty";

                var step = PlainTextReporter.CreateStep("When I send a POST request to Auth endpoint");
                step.Url = _lastUrl;
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var payload = jwtToken.Payload.SerializeToJson().ToLower();

            var sensitiveFields = new[] { "password", "secret", "creditcard", "ssn", "socialsecurity" };

            foreach (var field in sensitiveFields)
            {
                if (payload.Contains(field))
                {
                    var report = PlainTextReporter.CreateReport(_currentTestCase);
                    report.Assertion = "JWT payload sensitive data check";
                    report.Expected = "JWT payload should not contain sensitive fields";
                    report.Actual = $"JWT payload contains sensitive field '{field}'";

                    var step = PlainTextReporter.CreateStep("When I send a POST request to Auth endpoint");
                    step.Url = _lastUrl;
                    step.ErrorMessage = $"JWT payload contains sensitive field '{field}'";
                    report.BddSteps.Add(step);

                    PlainTextReporter.ThrowFailure(report);
                }
            }
        }

        public string GetAccessToken()
        {
            return _response?["accessToken"]?.ToString() ?? "";
        }

        public string GetRefreshToken()
        {
            return _response?["refreshToken"]?.ToString() ?? "";
        }

        public JObject GetRawResponse()
        {
            return _response!;
        }
    }
}

