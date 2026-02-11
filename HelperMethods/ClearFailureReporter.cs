using System;
using System.Collections.Generic;
using NUnit.Framework;
using Newtonsoft.Json.Linq;

namespace to_integrations.HelperMethods
{
    /// <summary>
    /// Helper to create and throw clear test failure reports
    /// Focused on readability for POs, BAs, and Developers
    /// </summary>
    public static class ClearFailureReporter
    {
        /// <summary>
        /// Creates and throws a failure report with clear structure
        /// </summary>
        public static void ThrowClearFailure(string testCaseName, string errorReason, string requestUrl, 
            string requestBody = "", string responseBody = "", int responseStatusCode = 0)
        {
            var report = new TestFailureReport
            {
                TestCaseName = testCaseName,
                ErrorReason = errorReason,
                RequestUrl = requestUrl,
                RequestBody = requestBody,
                ResponseBody = responseBody,
                ResponseStatusCode = responseStatusCode
            };

            report.PrintReport();
            Assert.Fail(errorReason);
        }

        /// <summary>
        /// Adds a BDD step to the report
        /// </summary>
        public static TestFailureReport CreateReport(string testCaseName)
        {
            return new TestFailureReport { TestCaseName = testCaseName };
        }

        /// <summary>
        /// Asserts a condition and fails with clear message if false
        /// </summary>
        public static void AssertClear(bool condition, string testCaseName, string errorReason, 
            string requestUrl, string requestBody = "", string responseBody = "", int responseStatusCode = 0)
        {
            if (!condition)
            {
                ThrowClearFailure(testCaseName, errorReason, requestUrl, requestBody, responseBody, responseStatusCode);
            }
        }

        /// <summary>
        /// Creates a BDD step description
        /// </summary>
        public static BddStep CreateStep(string stepDescription)
        {
            return new BddStep(stepDescription);
        }

        /// <summary>
        /// Formats an assertion failure reason in plain language
        /// </summary>
        public static string FormatFailureReason(string fieldName, string expectedValue, string actualValue)
        {
            return $"Field '{fieldName}' has incorrect value. Expected: {expectedValue}, Actual: {actualValue}";
        }

        /// <summary>
        /// Formats a missing field failure reason
        /// </summary>
        public static string FormatMissingFieldReason(string fieldName)
        {
            return $"Required field '{fieldName}' is missing or empty in response";
        }

        /// <summary>
        /// Formats an array count mismatch failure reason
        /// </summary>
        public static string FormatArrayCountReason(string arrayName, int expectedCount, int actualCount)
        {
            return $"Array '{arrayName}' has incorrect count. Expected: {expectedCount}, Actual: {actualCount}";
        }

        /// <summary>
        /// Formats a validation error reason
        /// </summary>
        public static string FormatValidationError(string fieldName, string expectedFormat, string actualValue)
        {
            return $"Field '{fieldName}' does not match expected format '{expectedFormat}'. Actual value: {actualValue}";
        }
    }
}
