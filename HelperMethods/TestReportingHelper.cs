using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace to_integrations.HelperMethods
{
    /// <summary>
    /// Provides enhanced test reporting capabilities with detailed failure information.
    /// Helps create clear, actionable error messages for POs, BAs, and Devs.
    /// </summary>
    public static class TestReportingHelper
    {
        /// <summary>
        /// Asserts a condition and provides detailed failure information with context
        /// </summary>
        public static void AssertWithContext(bool condition, string assertionName, string expectedDescription, 
            string actualDescription, string remediation = "")
        {
            if (!condition)
            {
                var message = BuildDetailedErrorMessage(assertionName, expectedDescription, actualDescription, remediation);
                Assert.Fail(message);
            }
        }

        /// <summary>
        /// Asserts that a collection contains an expected item with detailed reporting
        /// </summary>
        public static void AssertCollectionContainsWithContext(IEnumerable<dynamic> collection, 
            Func<dynamic, bool> predicate, string itemDescription, string collectionName, string remediation = "")
        {
            var hasItem = collection.Any(predicate);
            if (!hasItem)
            {
                var actualItems = string.Join(", ", collection.Take(5).Select(x => x.ToString()));
                var message = BuildDetailedErrorMessage(
                    $"'{collectionName}' collection validation",
                    $"At least one item matching: {itemDescription}",
                    $"No matching items found. Available items: [{actualItems}]",
                    remediation
                );
                Assert.Fail(message);
            }
        }

        /// <summary>
        /// Asserts that each item in a collection matches a predicate with detailed failure reporting
        /// </summary>
        public static void AssertAllItemsMatchWithContext(IEnumerable<dynamic> collection,
            Func<dynamic, bool> predicate, string fieldName, string expectedFormat, string remediation = "")
        {
            var items = collection.ToList();
            var failedItems = items.Where(item => !predicate(item)).ToList();

            if (failedItems.Any())
            {
                var failureDetails = string.Join("\n", failedItems.Take(3).Select((item, index) =>
                    $"  [{index + 1}] {fieldName}: {item}"));

                var message = BuildDetailedErrorMessage(
                    $"'{fieldName}' format validation",
                    $"All items should be {expectedFormat}",
                    $"{failedItems.Count} of {items.Count} items failed validation:\n{failureDetails}",
                    remediation
                );
                Assert.Fail(message);
            }
        }

        /// <summary>
        /// Asserts that a JSON response field exists and has expected type/value
        /// </summary>
        public static void AssertJsonFieldWithContext(JObject response, string fieldPath, 
            JTokenType expectedType, string fieldDescription = "")
        {
            var field = response.SelectToken(fieldPath);
            
            if (field == null)
            {
                var availableFields = string.Join(", ", response.Properties().Select(p => p.Name).Take(5));
                var message = BuildDetailedErrorMessage(
                    $"Response field '{fieldPath}' validation",
                    $"Field '{fieldPath}' should exist as {expectedType}",
                    $"Field not found. Available root fields: [{availableFields}]"
                );
                Assert.Fail(message);
            }

            if (field.Type != expectedType)
            {
                var message = BuildDetailedErrorMessage(
                    $"Response field type validation for '{fieldPath}'",
                    $"Field type should be {expectedType}",
                    $"Actual type: {field.Type}. Actual value: {field}"
                );
                Assert.Fail(message);
            }
        }

        /// <summary>
        /// Asserts that a JSON array contains items with required fields
        /// </summary>
        public static void AssertArrayItemsHaveFieldsWithContext(JArray array, string[] requiredFields, 
            string arrayName = "items")
        {
            if (array == null || array.Count == 0)
            {
                var message = BuildDetailedErrorMessage(
                    $"'{arrayName}' array validation",
                    $"Array should not be empty",
                    $"Array is null or empty"
                );
                Assert.Fail(message);
            }

            var itemsMissingFields = new List<string>();
            
            for (int i = 0; i < array.Count; i++)
            {
                var item = array[i];
                var missingFields = requiredFields.Where(f => item[f] == null).ToList();
                
                if (missingFields.Any())
                {
                    itemsMissingFields.Add($"Item {i}: missing [{string.Join(", ", missingFields)}]");
                }
            }

            if (itemsMissingFields.Any())
            {
                var message = BuildDetailedErrorMessage(
                    $"'{arrayName}' items field validation",
                    $"Each item should have fields: [{string.Join(", ", requiredFields)}]",
                    $"{itemsMissingFields.Count} of {array.Count} items are missing required fields:\n" +
                    string.Join("\n", itemsMissingFields.Take(3)),
                    "Verify API response schema matches documentation"
                );
                Assert.Fail(message);
            }
        }

        /// <summary>
        /// Asserts array property format (e.g., 2-char codes, 3-char codes, integers)
        /// </summary>
        public static void AssertArrayPropertyFormatWithContext(JArray array, string propertyName, 
            Func<string, bool> validator, string expectedFormat, string arrayName = "items")
        {
            if (array == null || array.Count == 0)
            {
                var message = BuildDetailedErrorMessage(
                    $"'{arrayName}' array validation",
                    $"Array should not be empty",
                    $"Array is null or empty"
                );
                Assert.Fail(message);
            }

            var invalidItems = new List<(int Index, string Value)>();
            
            for (int i = 0; i < array.Count; i++)
            {
                var value = array[i][propertyName]?.ToString();
                
                if (value != null && !validator(value))
                {
                    invalidItems.Add((i, value));
                }
            }

            if (invalidItems.Any())
            {
                var exampleInvalidItems = string.Join("\n", invalidItems.Take(3).Select(item =>
                    $"  Item {item.Index}: '{item.Value}' (expected {expectedFormat})"));

                var message = BuildDetailedErrorMessage(
                    $"'{arrayName}.{propertyName}' format validation",
                    $"All '{propertyName}' values should be {expectedFormat}",
                    $"{invalidItems.Count} of {array.Count} items have invalid format:\n{exampleInvalidItems}",
                    $"Review API documentation for {propertyName} field specification"
                );
                Assert.Fail(message);
            }
        }

        /// <summary>
        /// Asserts that one array is a subset of another with detailed reporting
        /// </summary>
        public static void AssertArrayIsSubsetWithContext(JArray parentArray, JArray subsetArray, 
            string identifierField, string parentDescription = "full result", string subsetDescription = "filtered result")
        {
            if (parentArray == null || subsetArray == null)
            {
                var message = BuildDetailedErrorMessage(
                    "Array subset validation",
                    $"Both arrays should not be null",
                    $"Parent array null: {parentArray == null}, Subset array null: {subsetArray == null}"
                );
                Assert.Fail(message);
            }

            var parentIds = parentArray
                .Select(item => item[identifierField]?.ToString())
                .Where(id => id != null)
                .ToHashSet();

            var subsetIds = subsetArray
                .Select(item => item[identifierField]?.ToString())
                .Where(id => id != null)
                .ToList();

            var missingIds = subsetIds.Where(id => !parentIds.Contains(id)).ToList();

            if (missingIds.Any())
            {
                var message = BuildDetailedErrorMessage(
                    "Array subset validation",
                    $"All items in '{subsetDescription}' should exist in '{parentDescription}'",
                    $"{missingIds.Count} items from '{subsetDescription}' are missing from '{parentDescription}':\n" +
                    $"Missing IDs: [{string.Join(", ", missingIds.Take(5))}]",
                    $"Verify API logic for filtering. {subsetDescription} should be a subset of {parentDescription}"
                );
                Assert.Fail(message);
            }
        }

        /// <summary>
        /// Creates a detailed, structured error message for test failures
        /// </summary>
        public static string BuildDetailedErrorMessage(string assertionName, string expected, 
            string actual, string remediation = "")
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("╔════════════════════════════════════════════════════════════════╗");
            sb.AppendLine("║                    TEST FAILURE DETAILS                         ║");
            sb.AppendLine("╚════════════════════════════════════════════════════════════════╝");
            sb.AppendLine();
            
            sb.AppendLine($"❌ ASSERTION: {assertionName}");
            sb.AppendLine();
            
            sb.AppendLine("📋 EXPECTED:");
            foreach (var line in expected.Split('\n'))
            {
                sb.AppendLine($"   {line}");
            }
            sb.AppendLine();
            
            sb.AppendLine("❌ ACTUAL:");
            foreach (var line in actual.Split('\n'))
            {
                sb.AppendLine($"   {line}");
            }
            sb.AppendLine();

            if (!string.IsNullOrEmpty(remediation))
            {
                sb.AppendLine("💡 REMEDIATION:");
                foreach (var line in remediation.Split('\n'))
                {
                    sb.AppendLine($"   • {line}");
                }
                sb.AppendLine();
            }

            sb.AppendLine("──────────────────────────────────────────────────────────────────");
            
            return sb.ToString();
        }

        /// <summary>
        /// Logs detailed request/response information for debugging
        /// </summary>
        public static void LogRequestResponseDetails(string method, string url, string headers, 
            string requestBody, int statusCode, string responseBody)
        {
            Console.WriteLine();
            Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    API REQUEST/RESPONSE LOG                     ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            
            Console.WriteLine($"📤 REQUEST");
            Console.WriteLine($"   Method: {method}");
            Console.WriteLine($"   URL: {url}");
            if (!string.IsNullOrEmpty(headers))
            {
                Console.WriteLine($"   Headers: {headers}");
            }
            if (!string.IsNullOrEmpty(requestBody))
            {
                Console.WriteLine($"   Body: {requestBody}");
            }
            Console.WriteLine();
            
            Console.WriteLine($"📥 RESPONSE");
            Console.WriteLine($"   Status: {statusCode}");
            Console.WriteLine($"   Body:");
            try
            {
                var beautified = JToken.Parse(responseBody).ToString(Newtonsoft.Json.Formatting.Indented);
                foreach (var line in beautified.Split('\n'))
                {
                    Console.WriteLine($"      {line}");
                }
            }
            catch
            {
                foreach (var line in responseBody.Split('\n'))
                {
                    Console.WriteLine($"      {line}");
                }
            }
            Console.WriteLine();
            Console.WriteLine("──────────────────────────────────────────────────────────────────");
            Console.WriteLine();
        }

        /// <summary>
        /// Formats and logs assertion context for better debugging
        /// </summary>
        public static void LogAssertionContext(Dictionary<string, object> context)
        {
            if (!context.Any()) return;

            Console.WriteLine();
            Console.WriteLine("📊 ASSERTION CONTEXT:");
            foreach (var kvp in context)
            {
                Console.WriteLine($"   {kvp.Key}: {kvp.Value}");
            }
            Console.WriteLine();
        }
    }
}
