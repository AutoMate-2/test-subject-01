using System;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using to_integrations.HelperMethods;

namespace to_integrations.CRUD.Catalogue.Api
{
    /// <summary>
    /// Example: How to use ClearFailureReporter for test failures
    /// This shows the pattern to use in your test assertions
    /// </summary>
    public class CatalogueCrudWithClearReporting
    {
        // Example 1: Simple assertion with clear reporting
        public void AssertResponseStatusCodeExample(int expectedStatus, int actualStatus, string requestUrl, string responseBody)
        {
            if (actualStatus != expectedStatus)
            {
                var errorReason = $"Response status code mismatch. Expected: {expectedStatus}, Actual: {actualStatus}";
                
                ClearFailureReporter.ThrowClearFailure(
                    testCaseName: "TC-TO-CAT-001 - Get destination countries",
                    errorReason: errorReason,
                    requestUrl: requestUrl,
                    responseBody: responseBody,
                    responseStatusCode: actualStatus
                );
            }
        }

        // Example 2: Array validation with clear reporting
        public void AssertCountriesArrayNotEmptyExample(JArray countries, string requestUrl, string responseBody)
        {
            if (countries == null || countries.Count == 0)
            {
                var errorReason = "Response does not contain countries array or array is empty";
                
                ClearFailureReporter.ThrowClearFailure(
                    testCaseName: "TC-TO-CAT-001 - Get destination countries",
                    errorReason: errorReason,
                    requestUrl: requestUrl,
                    responseBody: responseBody
                );
            }
        }

        // Example 3: Field validation with clear reporting
        public void AssertCountriesHaveFlightsExample(JArray countries, string requestUrl, string responseBody)
        {
            var hasFlightCountries = countries.Any(c => c["hasFlights"] != null && c["hasFlights"].Value<bool>());
            
            if (!hasFlightCountries)
            {
                var errorReason = "Response does not contain any countries with flight packages. The 'hasFlights' property is missing or all values are false";
                
                ClearFailureReporter.ThrowClearFailure(
                    testCaseName: "TC-TO-CAT-002 - Get destination countries for hotels and flights",
                    errorReason: errorReason,
                    requestUrl: requestUrl,
                    responseBody: responseBody,
                    responseStatusCode: 200
                );
            }
        }

        // Example 4: Field format validation with clear reporting
        public void AssertCountryCodesFormatExample(JArray countries, string requestUrl, string responseBody)
        {
            var invalidItems = new System.Collections.Generic.List<(int Index, string Code)>();
            
            for (int i = 0; i < countries.Count; i++)
            {
                var code = countries[i]["countryCode"]?.ToString();
                if (code == null || code.Length != 2)
                {
                    invalidItems.Add((i, code ?? "null"));
                }
            }

            if (invalidItems.Count > 0)
            {
                var sampleInvalid = string.Join(", ", invalidItems.Take(3).Select(x => $"Index {x.Index}: '{x.Code}'"));
                var errorReason = $"Country codes do not match ISO 3166-1 alpha-2 format (2 letters). Found {invalidItems.Count} invalid codes: {sampleInvalid}";
                
                ClearFailureReporter.ThrowClearFailure(
                    testCaseName: "TC-TO-CAT-005 - Verify country data format",
                    errorReason: errorReason,
                    requestUrl: requestUrl,
                    responseBody: responseBody,
                    responseStatusCode: 200
                );
            }
        }

        // Example 5: Missing required fields with clear reporting
        public void AssertCountriesHaveRequiredFieldsExample(JArray countries, string[] requiredFields, string requestUrl, string responseBody)
        {
            var itemsMissingFields = new System.Collections.Generic.List<string>();
            
            for (int i = 0; i < countries.Count; i++)
            {
                var missingFields = requiredFields.Where(f => countries[i][f] == null).ToList();
                if (missingFields.Count > 0)
                {
                    itemsMissingFields.Add($"Country at index {i}: missing [{string.Join(", ", missingFields)}]");
                }
            }

            if (itemsMissingFields.Count > 0)
            {
                var sampleMissing = string.Join("\n", itemsMissingFields.Take(3));
                var errorReason = $"Some countries are missing required fields. Expected fields: [{string.Join(", ", requiredFields)}]\n\nMissing in:\n{sampleMissing}";
                
                ClearFailureReporter.ThrowClearFailure(
                    testCaseName: "TC-TO-CAT-001 - Get destination countries for hotels only",
                    errorReason: errorReason,
                    requestUrl: requestUrl,
                    responseBody: responseBody,
                    responseStatusCode: 200
                );
            }
        }

        // Example 6: Subset validation with clear reporting
        public void AssertArrayIsSubsetExample(JArray parentArray, JArray subsetArray, string parentName, string subsetName, string requestUrl, string responseBody)
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
                var sampleMissing = string.Join(", ", missingIds.Take(5));
                var errorReason = $"Some items from '{subsetName}' response are missing from '{parentName}' response. Missing {missingIds.Count} countries: {sampleMissing}";
                
                ClearFailureReporter.ThrowClearFailure(
                    testCaseName: "TC-TO-CAT-006 - Verify difference between onlyHotels true and false",
                    errorReason: errorReason,
                    requestUrl: requestUrl,
                    responseBody: responseBody,
                    responseStatusCode: 200
                );
            }
        }
    }
}
