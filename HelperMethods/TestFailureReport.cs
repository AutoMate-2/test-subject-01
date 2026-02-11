using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace to_integrations.HelperMethods
{
    /// <summary>
    /// Generates clear, structured test failure reports for failed test cases.
    /// Format is optimized for POs, BAs, and Developers to quickly understand failures.
    /// </summary>
    public class TestFailureReport
    {
        public string TestCaseName { get; set; }
        public string ErrorReason { get; set; }
        public List<BddStep> BddSteps { get; set; }
        public string RequestUrl { get; set; }
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
        public int ResponseStatusCode { get; set; }
        public DateTime FailureTime { get; set; }

        public TestFailureReport()
        {
            BddSteps = new List<BddStep>();
            FailureTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Generates the formatted failure report as a string
        /// </summary>
        public string GenerateReport()
        {
            var sb = new StringBuilder();

            // Test Case Name
            sb.AppendLine("TestCase: " + TestCaseName);
            sb.AppendLine();

            // Error Reason (plain, no assert messages)
            sb.AppendLine("Error: " + ErrorReason);
            sb.AppendLine();

            // BDD Steps
            sb.AppendLine("BDD Steps:");
            foreach (var step in BddSteps)
            {
                sb.AppendLine(step.Description);
                
                if (!string.IsNullOrEmpty(step.Url))
                {
                    sb.AppendLine("URL: " + step.Url);
                }
                
                if (!string.IsNullOrEmpty(step.RequestBody))
                {
                    sb.AppendLine("Request Body:");
                    sb.AppendLine(FormatJson(step.RequestBody));
                }
                
                if (!string.IsNullOrEmpty(step.ResponseBody))
                {
                    sb.AppendLine("Response Body:");
                    sb.AppendLine(FormatJson(step.ResponseBody));
                }
                
                sb.AppendLine();
            }

            // Request details (if available)
            if (!string.IsNullOrEmpty(RequestUrl))
            {
                sb.AppendLine("Request URL: " + RequestUrl);
                sb.AppendLine();
            }

            if (!string.IsNullOrEmpty(RequestBody))
            {
                sb.AppendLine("Request Body:");
                sb.AppendLine(FormatJson(RequestBody));
                sb.AppendLine();
            }

            // Response details (if available)
            if (ResponseStatusCode > 0)
            {
                sb.AppendLine("Response Status Code: " + ResponseStatusCode);
                sb.AppendLine();
            }

            if (!string.IsNullOrEmpty(ResponseBody))
            {
                sb.AppendLine("Response Body:");
                sb.AppendLine(FormatJson(ResponseBody));
                sb.AppendLine();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Formats JSON string for readability
        /// </summary>
        private string FormatJson(string json)
        {
            if (string.IsNullOrEmpty(json))
                return "(empty)";

            try
            {
                var parsed = JToken.Parse(json);
                return parsed.ToString(Newtonsoft.Json.Formatting.Indented);
            }
            catch
            {
                // If not valid JSON, return as-is
                return json;
            }
        }

        /// <summary>
        /// Prints the report to console
        /// </summary>
        public void PrintReport()
        {
            Console.WriteLine(GenerateReport());
        }

        /// <summary>
        /// Creates a report and throws with the formatted message
        /// </summary>
        public void FailWithReport()
        {
            throw new Exception(GenerateReport());
        }
    }

    /// <summary>
    /// Represents a single BDD step in the test
    /// </summary>
    public class BddStep
    {
        public string Description { get; set; }
        public string Url { get; set; }
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
        public int? ResponseStatusCode { get; set; }

        public BddStep(string description)
        {
            Description = description;
        }
    }
}
