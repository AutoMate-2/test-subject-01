using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Linq;
using to_integrations.HelperMethods;

namespace to_integrations.CRUD.Catalogue.Api
{
    public class CatalogueCrud
    {
        private readonly CatalogueApi _api = new CatalogueApi();
        private JObject? _response;
        private string _lastUrl = "";
        private string _lastRequestBody = "";
        private string _currentTestCase = "";

        public int LastStatus { get; private set; }
        public string LastBody { get; private set; } = "";

        public void SetTestCase(string testCase)
        {
            _currentTestCase = testCase;
        }

        public async Task GetDestinationCountriesAsync(string queryString = "", string token = "")
        {
            var sw = Stopwatch.StartNew();
            var (status, body) = await _api.GetDestinationCountriesAsync(queryString, token);
            sw.Stop();

            LastStatus = status;
            LastBody = body;
            _lastUrl = $"GET https://integrations-main-nws.nugios.test/api/Catalogue/GetDestinationCountries{queryString}";
            _lastRequestBody = "(empty)";

            LogResponse(status, body);
            ParseResponse(body);
        }

        private void LogResponse(int status, string body)
        {
            Console.WriteLine("====== CATALOGUE RESPONSE ======");
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
            
            Console.WriteLine("================================");
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

        public void AssertStatusCodeIs(int expectedCode)
        {
            if (LastStatus != expectedCode)
            {
                var report = PlainTextReporter.CreateReport(_currentTestCase);
                report.Assertion = "Response status code validation";
                report.Expected = $"Status code should be {expectedCode}";
                report.Actual = $"Status code is {LastStatus}";

                var step = PlainTextReporter.CreateStep("When I send a GET request");
                step.Url = _lastUrl;
                step.ErrorMessage = $"Expected status {expectedCode} but got {LastStatus}";
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }
        }

        public void AssertResponseContainsArray(string fieldName)
        {
            var field = _response?[fieldName];
            
            if (field == null || field.Type != JTokenType.Array)
            {
                var report = PlainTextReporter.CreateReport(_currentTestCase);
                report.Assertion = $"Response array validation for '{fieldName}'";
                report.Expected = $"Response should contain '{fieldName}' array";
                report.Actual = field == null ? $"Field '{fieldName}' is missing" : $"Field '{fieldName}' is not an array";

                var step = PlainTextReporter.CreateStep("When I send a GET request");
                step.Url = _lastUrl;
                step.ErrorMessage = field == null ? $"Response does not contain '{fieldName}'" : $"Field '{fieldName}' is not an array";
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }
        }

        public void AssertResponseFieldIsFalse(string fieldName)
        {
            var field = _response?[fieldName];
            
            if (field == null || field.Type != JTokenType.Boolean || field.Value<bool>() != false)
            {
                var report = PlainTextReporter.CreateReport(_currentTestCase);
                report.Assertion = $"Response field '{fieldName}' validation";
                report.Expected = $"Field '{fieldName}' should be false";
                report.Actual = field == null ? $"Field '{fieldName}' is missing" : 
                    field.Type != JTokenType.Boolean ? $"Field '{fieldName}' is not a boolean" : 
                    $"Field '{fieldName}' is {field.Value<bool>()}";

                var step = PlainTextReporter.CreateStep("When I send a GET request");
                step.Url = _lastUrl;
                step.ErrorMessage = $"Field '{fieldName}' is not false";
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }
        }

        public void AssertCountriesHaveRequiredFields(string[] requiredFields)
        {
            var countries = _response?["countries"] as JArray;
            
            if (countries == null || countries.Count == 0)
            {
                var report = PlainTextReporter.CreateReport(_currentTestCase);
                report.Assertion = "Countries array validation";
                report.Expected = "Countries array should not be empty";
                report.Actual = countries == null ? "Countries array is null" : "Countries array is empty";

                var step = PlainTextReporter.CreateStep("When I send a GET request");
                step.Url = _lastUrl;
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }

            var itemsMissing = new System.Collections.Generic.List<string>();
            for (int i = 0; i < countries!.Count; i++)
            {
                var missing = requiredFields.Where(f => countries[i][f] == null).ToList();
                if (missing.Count > 0)
                {
                    itemsMissing.Add($"Country at index {i}: missing [{string.Join(", ", missing)}]");
                }
            }

            if (itemsMissing.Count > 0)
            {
                var samples = string.Join("\n", itemsMissing.Take(3));
                
                var report = PlainTextReporter.CreateReport(_currentTestCase);
                report.Assertion = "Required fields validation";
                report.Expected = $"Each country should have: [{string.Join(", ", requiredFields)}]";
                report.Actual = $"Found {itemsMissing.Count} countries missing fields:\n{samples}";

                var step = PlainTextReporter.CreateStep("When I send a GET request");
                step.Url = _lastUrl;
                step.ErrorMessage = "Some countries are missing required fields";
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }
        }

        public void AssertResponseContainsCountriesWithFlights()
        {
            var countries = _response?["countries"] as JArray;
            
            if (countries == null || countries.Count == 0)
            {
                var report = PlainTextReporter.CreateReport(_currentTestCase);
                report.Assertion = "Countries array validation";
                report.Expected = "Countries array should not be empty";
                report.Actual = countries == null ? "Countries array is null" : "Countries array is empty";

                var step = PlainTextReporter.CreateStep("When I send a GET request");
                step.Url = _lastUrl;
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }

            var countriesWithFlights = countries!
                .Where(c => c["hasFlights"] != null && c["hasFlights"].Value<bool>())
                .ToList();

            if (countriesWithFlights.Count == 0)
            {
                var report = PlainTextReporter.CreateReport(_currentTestCase);
                report.Assertion = "Flight packages availability check";
                report.Expected = "At least one country should have 'hasFlights' field set to true";
                report.Actual = $"No countries with flight found in {countries.Count} countries";

                var step = PlainTextReporter.CreateStep("When I send a GET request");
                step.Url = _lastUrl;
                step.ErrorMessage = "Response does not contain any countries with flight packages";
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }
        }

        public void AssertCountryCodeFormat()
        {
            var countries = _response?["countries"] as JArray;
            
            if (countries == null || countries.Count == 0)
            {
                var report = PlainTextReporter.CreateReport(_currentTestCase);
                report.Assertion = "Countries array validation";
                report.Expected = "Countries array should not be empty";
                report.Actual = countries == null ? "Countries array is null" : "Countries array is empty";

                var step = PlainTextReporter.CreateStep("When I send a GET request");
                step.Url = _lastUrl;
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }

            var invalidCodes = new System.Collections.Generic.List<(int Index, string Code)>();

            for (int i = 0; i < countries!.Count; i++)
            {
                var code = countries[i]["countryCode"]?.ToString();
                if (code == null || code.Length != 2 || !code.All(char.IsLetter))
                {
                    invalidCodes.Add((i, code ?? "null"));
                }
            }

            if (invalidCodes.Count > 0)
            {
                var samples = string.Join(", ", invalidCodes.Take(3).Select(x => $"[{x.Index}]: {x.Code}"));
                
                var report = PlainTextReporter.CreateReport(_currentTestCase);
                report.Assertion = "Country code format validation";
                report.Expected = "Each country code should be 2 letters (ISO 3166-1 alpha-2)";
                report.Actual = $"Found {invalidCodes.Count} invalid country codes. Examples: {samples}";

                var step = PlainTextReporter.CreateStep("When I send a GET request");
                step.Url = _lastUrl;
                step.ErrorMessage = $"Country code format invalid. Found: {samples}";
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }
        }

        public void AssertCountryISO3Format()
        {
            var countries = _response?["countries"] as JArray;
            
            if (countries == null || countries.Count == 0)
            {
                var report = PlainTextReporter.CreateReport(_currentTestCase);
                report.Assertion = "Countries array validation";
                report.Expected = "Countries array should not be empty";
                report.Actual = countries == null ? "Countries array is null" : "Countries array is empty";

                var step = PlainTextReporter.CreateStep("When I send a GET request");
                step.Url = _lastUrl;
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }

            var invalidCodes = new System.Collections.Generic.List<(int Index, string Code)>();

            for (int i = 0; i < countries!.Count; i++)
            {
                var code = countries[i]["isO3"]?.ToString();
                if (code == null || code.Length != 3 || !code.All(char.IsLetter))
                {
                    invalidCodes.Add((i, code ?? "null"));
                }
            }

            if (invalidCodes.Count > 0)
            {
                var samples = string.Join(", ", invalidCodes.Take(3).Select(x => $"[{x.Index}]: {x.Code}"));
                
                var report = PlainTextReporter.CreateReport(_currentTestCase);
                report.Assertion = "ISO3 code format validation";
                report.Expected = "Each ISO3 code should be 3 letters (ISO 3166-1 alpha-3)";
                report.Actual = $"Found {invalidCodes.Count} invalid ISO3 codes. Examples: {samples}";

                var step = PlainTextReporter.CreateStep("When I send a GET request");
                step.Url = _lastUrl;
                step.ErrorMessage = $"ISO3 code format invalid. Found: {samples}";
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }
        }

        public void AssertSellCurrencyFormat()
        {
            var countries = _response?["countries"] as JArray;
            
            if (countries == null || countries.Count == 0)
            {
                var report = PlainTextReporter.CreateReport(_currentTestCase);
                report.Assertion = "Countries array validation";
                report.Expected = "Countries array should not be empty";
                report.Actual = countries == null ? "Countries array is null" : "Countries array is empty";

                var step = PlainTextReporter.CreateStep("When I send a GET request");
                step.Url = _lastUrl;
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }

            var invalidCodes = new System.Collections.Generic.List<(int Index, string Code)>();

            for (int i = 0; i < countries!.Count; i++)
            {
                var code = countries[i]["sellCurrency"]?.ToString();
                if (code == null || code.Length != 3 || !code.All(char.IsLetter))
                {
                    invalidCodes.Add((i, code ?? "null"));
                }
            }

            if (invalidCodes.Count > 0)
            {
                var samples = string.Join(", ", invalidCodes.Take(3).Select(x => $"[{x.Index}]: {x.Code}"));
                
                var report = PlainTextReporter.CreateReport(_currentTestCase);
                report.Assertion = "Currency code format validation";
                report.Expected = "Each currency code should be 3 letters (ISO 4217)";
                report.Actual = $"Found {invalidCodes.Count} invalid currency codes. Examples: {samples}";

                var step = PlainTextReporter.CreateStep("When I send a GET request");
                step.Url = _lastUrl;
                step.ErrorMessage = $"Currency code format invalid. Found: {samples}";
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }
        }

        public void AssertStatusIsInteger()
        {
            var countries = _response?["countries"] as JArray;
            
            if (countries == null || countries.Count == 0)
            {
                var report = PlainTextReporter.CreateReport(_currentTestCase);
                report.Assertion = "Countries array validation";
                report.Expected = "Countries array should not be empty";
                report.Actual = countries == null ? "Countries array is null" : "Countries array is empty";

                var step = PlainTextReporter.CreateStep("When I send a GET request");
                step.Url = _lastUrl;
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }

            var invalid = new System.Collections.Generic.List<(int Index, object Value)>();

            for (int i = 0; i < countries!.Count; i++)
            {
                var status = countries[i]["status"];
                if (status?.Type != JTokenType.Integer)
                {
                    invalid.Add((i, status?.ToString() ?? "null"));
                }
            }

            if (invalid.Count > 0)
            {
                var samples = string.Join(", ", invalid.Take(3).Select(x => $"[{x.Index}]: {x.Value}"));
                
                var report = PlainTextReporter.CreateReport(_currentTestCase);
                report.Assertion = "Status field type validation";
                report.Expected = "Each status field should be an integer";
                report.Actual = $"Found {invalid.Count} non-integer status values. Examples: {samples}";

                var step = PlainTextReporter.CreateStep("When I send a GET request");
                step.Url = _lastUrl;
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }
        }

        public void AssertSortingOrderIsInteger()
        {
            var countries = _response?["countries"] as JArray;
            
            if (countries == null || countries.Count == 0)
            {
                var report = PlainTextReporter.CreateReport(_currentTestCase);
                report.Assertion = "Countries array validation";
                report.Expected = "Countries array should not be empty";
                report.Actual = countries == null ? "Countries array is null" : "Countries array is empty";

                var step = PlainTextReporter.CreateStep("When I send a GET request");
                step.Url = _lastUrl;
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }

            var invalid = new System.Collections.Generic.List<(int Index, object Value)>();

            for (int i = 0; i < countries!.Count; i++)
            {
                var order = countries[i]["sortingOrder"];
                if (order?.Type != JTokenType.Integer)
                {
                    invalid.Add((i, order?.ToString() ?? "null"));
                }
            }

            if (invalid.Count > 0)
            {
                var samples = string.Join(", ", invalid.Take(3).Select(x => $"[{x.Index}]: {x.Value}"));
                
                var report = PlainTextReporter.CreateReport(_currentTestCase);
                report.Assertion = "Sorting order field type validation";
                report.Expected = "Each sortingOrder field should be an integer";
                report.Actual = $"Found {invalid.Count} non-integer sortingOrder values. Examples: {samples}";

                var step = PlainTextReporter.CreateStep("When I send a GET request");
                step.Url = _lastUrl;
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }
        }

        public JArray GetCountries()
        {
            var countries = _response?["countries"] as JArray;
            
            if (countries == null || countries.Count == 0)
            {
                var report = PlainTextReporter.CreateReport(_currentTestCase);
                report.Assertion = "Get countries array";
                report.Expected = "Countries array should exist and not be empty";
                report.Actual = countries == null ? "Countries array is null" : "Countries array is empty";

                var step = PlainTextReporter.CreateStep("When I get countries from response");
                step.Url = _lastUrl;
                step.ErrorMessage = "Countries array is null or empty";
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }
            
            return countries!;
        }

        public void AssertResponseContainsActivityId()
        {
            var activityId = _response?["activityId"];
            
            if (activityId == null)
            {
                var report = PlainTextReporter.CreateReport(_currentTestCase);
                report.Assertion = "Activity ID presence validation";
                report.Expected = "Response should contain 'activityId' field for tracing";
                report.Actual = "Field 'activityId' is missing";

                var step = PlainTextReporter.CreateStep("When I send a GET request");
                step.Url = _lastUrl;
                step.ErrorMessage = "Response does not contain 'activityId'";
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }
        }

        public JObject GetRawResponse()
        {
            return _response!;
        }
    }
}
