using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace to_integrations.HelperMethods
{
    /// <summary>
    /// Plain-text test failure reporter with clean, simple output format.
    /// No emojis, no box drawings, just clear information.
    /// </summary>
    public class PlainTextFailureReport
    {
        public string TestCase { get; set; }
        public string Assertion { get; set; }
        public string Expected { get; set; }
        public string Actual { get; set; }
        public List<BddStepSimple> BddSteps { get; set; }

        public PlainTextFailureReport()
        {
            BddSteps = new List<BddStepSimple>();
        }

        /// <summary>
        /// Generates the plain-text failure report
        /// </summary>
        public string GenerateReport()
        {
            var sb = new StringBuilder();

            // Test Case
            sb.AppendLine("Test case: " + TestCase);
            sb.AppendLine();

            // Assertion
            if (!string.IsNullOrEmpty(Assertion))
            {
                sb.AppendLine("Assertion: " + Assertion);
            }

            // Expected
            if (!string.IsNullOrEmpty(Expected))
            {
                sb.AppendLine("Expected: " + Expected);
            }

            // Actual
            if (!string.IsNullOrEmpty(Actual))
            {
                sb.AppendLine("Actual: " + Actual);
            }

            // Error message from the first step (if any)
            if (BddSteps.Count > 0 && !string.IsNullOrEmpty(BddSteps[0].ErrorMessage))
            {
                sb.AppendLine();
                sb.AppendLine("Error: " + BddSteps[0].ErrorMessage);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Throws exception with full report as message
        /// </summary>
        public void FailAndThrow()
        {
            var fullReport = GenerateReport();
            Assert.Fail(fullReport);
        }

        /// <summary>
        /// Formats JSON for readability
        /// </summary>
        private string FormatJson(string json)
        {
            if (string.IsNullOrEmpty(json) || json.Equals("(empty)"))
                return "(empty)";

            try
            {
                var parsed = JToken.Parse(json);
                return parsed.ToString(Newtonsoft.Json.Formatting.Indented);
            }
            catch
            {
                return json;
            }
        }
    }

    /// <summary>
    /// Represents a single BDD step
    /// </summary>
    public class BddStepSimple
    {
        public string Description { get; set; }
        public string Url { get; set; }
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
        public string ErrorMessage { get; set; }

        public BddStepSimple(string description)
        {
            Description = description;
        }
    }

    /// <summary>
    /// Helper methods to create and throw clear, plain-text failures
    /// </summary>
    public static class PlainTextReporter
    {
        /// <summary>
        /// Creates a new failure report
        /// </summary>
        public static PlainTextFailureReport CreateReport(string testCase)
        {
            return new PlainTextFailureReport { TestCase = testCase };
        }

        /// <summary>
        /// Creates a BDD step
        /// </summary>
        public static BddStepSimple CreateStep(string description)
        {
            return new BddStepSimple(description);
        }

        /// <summary>
        /// Throws a failure with plain-text report
        /// </summary>
        public static void ThrowFailure(PlainTextFailureReport report)
        {
            report.FailAndThrow();
        }

        /// <summary>
        /// Quick throw without building full report
        /// </summary>
        public static void QuickThrow(string testCase, string assertion, string expected, string actual)
        {
            var report = new PlainTextFailureReport
            {
                TestCase = testCase,
                Assertion = assertion,
                Expected = expected,
                Actual = actual
            };
            report.FailAndThrow();
        }
    }
}
