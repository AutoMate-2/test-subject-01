using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using to_integrations.HelperMethods;

namespace to_integrations.CRUD.Catalogue.Api
{
    /// <summary>
    /// Examples: How to use PlainTextFailureReport for test failures
    /// Clean, plain-text output with no emojis or complex formatting
    /// </summary>
    public class CatalogueCrudWithPlainTextReporting
    {
        // Example 1: Simple status code check
        public void AssertStatusCodeExample(int expectedStatus, int actualStatus, 
            string url, string responseBody)
        {
            if (actualStatus != expectedStatus)
            {
                var report = PlainTextReporter.CreateReport("TC_TO_CAT_001_GetDestinationCountries");
                report.Assertion = "Response status code validation";
                report.Expected = $"Status code should be {expectedStatus}";
                report.Actual = $"Status code is {actualStatus}";

                var step = PlainTextReporter.CreateStep("When I send a GET request to \"/api/Catalogue/GetDestinationCountries?onlyHotels=true\"");
                step.Url = "GET " + url;
                step.ResponseBody = responseBody;
                step.ErrorMessage = $"Expected status {expectedStatus} but got {actualStatus}";

                report.BddSteps.Add(step);
                PlainTextReporter.ThrowFailure(report);
            }
        }

        // Example 2: Array empty check
        public void AssertCountriesArrayNotEmptyExample(JArray countries, string url, string responseBody)
        {
            if (countries == null || countries.Count == 0)
            {
                var report = PlainTextReporter.CreateReport("TC_TO_CAT_001_GetDestinationCountries");
                report.Assertion = "Countries array validation";
                report.Expected = "Response should contain countries array with items";
                report.Actual = "Countries array is empty or null";

                var step = PlainTextReporter.CreateStep("When I send a GET request to \"/api/Catalogue/GetDestinationCountries?onlyHotels=true\"");
                step.Url = "GET " + url;
                step.ResponseBody = responseBody;
                step.ErrorMessage = "Response does not contain countries array";

                report.BddSteps.Add(step);
                PlainTextReporter.ThrowFailure(report);
            }
        }

        // Example 3: Flight packages check (the one from your error)
        public void AssertCountriesHaveFlightsExample(JArray countries, string url, string responseBody)
        {
            var countriesWithFlights = countries
                .Where(c => c["hasFlights"] != null && c["hasFlights"].Value<bool>())
                .ToList();

            if (countriesWithFlights.Count == 0)
            {
                var report = PlainTextReporter.CreateReport("TC_TO_CAT_002_GetDestinationCountriesForHotelsAndFlights");
                report.Assertion = "Flight packages availability check";
                report.Expected = "At least one country should have 'hasFlights' field set to true";
                report.Actual = $"No countries with flight found in {countries.Count} countries";

                var step1 = PlainTextReporter.CreateStep("Given I am authenticated with valid token");
                report.BddSteps.Add(step1);

                var step2 = PlainTextReporter.CreateStep("When I send a GET request to \"/api/Catalogue/GetDestinationCountries?onlyHotels=false\"");
                step2.Url = "GET " + url;
                step2.RequestBody = "(empty)";
                step2.ResponseBody = responseBody;
                report.BddSteps.Add(step2);

                var step3 = PlainTextReporter.CreateStep("Then the response should contain countries with flight packages");
                step3.ErrorMessage = "Response does not contain any countries with flight packages. The 'hasFlights' property is missing or all values are false";
                report.BddSteps.Add(step3);

                PlainTextReporter.ThrowFailure(report);
            }
        }

        // Example 4: Country code format validation
        public void AssertCountryCodesFormatExample(JArray countries, string url, string responseBody)
        {
            var invalidCodes = new List<(int Index, string Code)>();

            for (int i = 0; i < countries.Count; i++)
            {
                var code = countries[i]["countryCode"]?.ToString();
                if (code == null || code.Length != 2 || !code.All(char.IsLetter))
                {
                    invalidCodes.Add((i, code ?? "null"));
                }
            }

            if (invalidCodes.Count > 0)
            {
                var sampleInvalid = string.Join(", ", invalidCodes.Take(3).Select(x => $"[{x.Index}]: {x.Code}"));
                
                var report = PlainTextReporter.CreateReport("TC_TO_CAT_005_VerifyCountryDataFormat");
                report.Assertion = "Country code format validation";
                report.Expected = "Each country code should be 2 letters (ISO 3166-1 alpha-2)";
                report.Actual = $"Found {invalidCodes.Count} invalid country codes. Examples: {sampleInvalid}";

                var step = PlainTextReporter.CreateStep("When I send a GET request to \"/api/Catalogue/GetDestinationCountries?onlyHotels=true\"");
                step.Url = "GET " + url;
                step.ResponseBody = responseBody;
                step.ErrorMessage = $"Country code format invalid. Found: {sampleInvalid}";

                report.BddSteps.Add(step);
                PlainTextReporter.ThrowFailure(report);
            }
        }

        // Example 5: Required fields check
        public void AssertCountriesHaveRequiredFieldsExample(JArray countries, string[] requiredFields, 
            string url, string responseBody)
        {
            var itemsMissing = new List<string>();

            for (int i = 0; i < countries.Count; i++)
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
                
                var report = PlainTextReporter.CreateReport("TC_TO_CAT_001_GetDestinationCountriesForHotelsOnly");
                report.Assertion = "Required fields validation";
                report.Expected = $"Each country should have fields: [{string.Join(", ", requiredFields)}]";
                report.Actual = $"Found {itemsMissing.Count} countries missing required fields:\n{samples}";

                var step = PlainTextReporter.CreateStep("When I send a GET request to \"/api/Catalogue/GetDestinationCountries?onlyHotels=true\"");
                step.Url = "GET " + url;
                step.ResponseBody = responseBody;
                step.ErrorMessage = $"Some countries are missing required fields";

                report.BddSteps.Add(step);
                PlainTextReporter.ThrowFailure(report);
            }
        }

        // Example 6: Array subset validation
        public void AssertArrayIsSubsetExample(JArray parentArray, JArray subsetArray, string parentName,
            string subsetName, string url, string responseBody)
        {
            var parentIds = parentArray
                .Select(item => item["countryID"]?.ToString())
                .Where(id => id != null)
                .ToHashSet();

            var subsetIds = subsetArray
                .Select(item => item["countryID"]?.ToString())
                .Where(id => id != null)
                .ToList();

            var missingIds = subsetIds.Where(id => !parentIds.Contains(id)).ToList();

            if (missingIds.Count > 0)
            {
                var missing = string.Join(", ", missingIds.Take(5));
                
                var report = PlainTextReporter.CreateReport("TC_TO_CAT_006_VerifyDifferenceBetweenOnlyHotels");
                report.Assertion = "Array subset validation";
                report.Expected = $"All items from '{subsetName}' should exist in '{parentName}'";
                report.Actual = $"Found {missingIds.Count} countries in '{subsetName}' missing from '{parentName}': {missing}";

                var step = PlainTextReporter.CreateStep("When I compare responses");
                step.Url = "GET " + url;
                step.ErrorMessage = $"Some countries are missing";

                report.BddSteps.Add(step);
                PlainTextReporter.ThrowFailure(report);
            }
        }
    }
}
