using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using Reqnroll;
using ToIntegrations.CRUD.Areas;
using to_integrations.HelperMethods;
using ToIntegrations.Models;

namespace ToIntegrations.Steps
{
    [Binding]
    public class AreasSteps
    {
        private readonly ScenarioContext _scenarioContext;

        public AreasSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"valid agent credentials")]
        public void GivenValidAgentCredentials()
        {
            var agentId = AppConfig.GetValue("AgentId") ?? "username";
            var agentPassword = AppConfig.GetValue("AgentPassword") ?? "password";
            Assert.IsNotEmpty(agentId, "AgentId must be configured");
            Assert.IsNotEmpty(agentPassword, "AgentPassword must be configured");
            _scenarioContext["AgentId"] = agentId;
            _scenarioContext["AgentPassword"] = agentPassword;
            TestContext.Progress.WriteLine("Valid agent credentials are available");
        }

        // Removed ambiguous generic GET request step definition

        [Then(@"for each item in response (.*)")]
        public void ThenForEachItemInResponse(string arrayName)
        {
            var content = _scenarioContext["GenericResponseContent"] as string;
            Assert.IsNotNull(content, "Response content was not captured");

            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            Assert.IsTrue(root.TryGetProperty(arrayName, out var dataArray), $"Response does not contain '{arrayName}' property");
            Assert.AreEqual(JsonValueKind.Array, dataArray.ValueKind, $"'{arrayName}' is not an array");

            var items = new List<JsonElement>();
            foreach (var item in dataArray.EnumerateArray())
            {
                items.Add(item.Clone());
            }

            Assert.Greater(items.Count, 0, $"'{arrayName}' array is empty");
            _scenarioContext["DataArrayItems"] = items;
            TestContext.Progress.WriteLine($"Found {items.Count} items in '{arrayName}' array");
        }

        [Then(@"the field (.*) must match GUID format")]
        public void ThenTheFieldMustMatchGUIDFormat(string fieldName)
        {
            var items = _scenarioContext["DataArrayItems"] as List<JsonElement>;
            Assert.IsNotNull(items, "Data array items were not captured");

            int index = 0;
            foreach (var item in items)
            {
                Assert.IsTrue(item.TryGetProperty(fieldName, out var fieldValue), $"Item at index {index} is missing field '{fieldName}'");
                
                var fieldString = fieldValue.GetString();
                Assert.IsNotNull(fieldString, $"Item at index {index} has null '{fieldName}'");
                Assert.IsNotEmpty(fieldString, $"Item at index {index} has empty '{fieldName}'");
                Assert.IsTrue(Guid.TryParse(fieldString, out _), $"Item at index {index}: '{fieldName}' value '{fieldString}' is not a valid GUID");
                
                index++;
            }

            TestContext.Progress.WriteLine($"All {items.Count} items have valid GUID format for field '{fieldName}'");
        }

        [Then(@"the field (.*) must not be null")]
        public void ThenTheFieldMustNotBeNull(string fieldName)
        {
            var items = _scenarioContext["DataArrayItems"] as List<JsonElement>;
            Assert.IsNotNull(items, "Data array items were not captured");

            int index = 0;
            foreach (var item in items)
            {
                Assert.IsTrue(item.TryGetProperty(fieldName, out var fieldValue), $"Item at index {index} is missing field '{fieldName}'");
                Assert.AreNotEqual(JsonValueKind.Null, fieldValue.ValueKind, $"Item at index {index} has null '{fieldName}'");
                
                var fieldString = fieldValue.GetString();
                Assert.IsNotNull(fieldString, $"Item at index {index} has null '{fieldName}' value");
                
                index++;
            }

            TestContext.Progress.WriteLine($"All {items.Count} items have non-null '{fieldName}' field");
        }

        [Then(@"the field (.*) must not be empty")]
        public void ThenTheFieldMustNotBeEmpty(string fieldName)
        {
            var items = _scenarioContext["DataArrayItems"] as List<JsonElement>;
            Assert.IsNotNull(items, "Data array items were not captured");

            int index = 0;
            foreach (var item in items)
            {
                Assert.IsTrue(item.TryGetProperty(fieldName, out var fieldValue), $"Item at index {index} is missing field '{fieldName}'");
                
                var fieldString = fieldValue.GetString();
                Assert.IsNotNull(fieldString, $"Item at index {index} has null '{fieldName}'");
                Assert.IsNotEmpty(fieldString, $"Item at index {index} has empty '{fieldName}'");
                
                index++;
            }

            TestContext.Progress.WriteLine($"All {items.Count} items have non-empty '{fieldName}' field");
        }

        [Then(@"all (.*) values in response (.*) must be unique")]
        public void ThenAllValuesInResponseMustBeUnique(string fieldName, string arrayName)
        {
            var content = _scenarioContext["GenericResponseContent"] as string;
            Assert.IsNotNull(content, "Response content was not captured");

            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            Assert.IsTrue(root.TryGetProperty(arrayName, out var dataArray), $"Response does not contain '{arrayName}' property");
            Assert.AreEqual(JsonValueKind.Array, dataArray.ValueKind, $"'{arrayName}' is not an array");

            var values = new List<string>();
            int index = 0;
            foreach (var item in dataArray.EnumerateArray())
            {
                Assert.IsTrue(item.TryGetProperty(fieldName, out var fieldValue), $"Item at index {index} is missing field '{fieldName}'");
                var fieldString = fieldValue.GetString();
                Assert.IsNotNull(fieldString, $"Item at index {index} has null '{fieldName}'");
                values.Add(fieldString);
                index++;
            }

            var duplicates = values.GroupBy(v => v).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            Assert.IsEmpty(duplicates, $"Found duplicate '{fieldName}' values: {string.Join(", ", duplicates)}");
            TestContext.Progress.WriteLine($"All {values.Count} '{fieldName}' values are unique");
        }

        [Then(@"the response time must be less than (\d+) milliseconds")]
        public void ThenTheResponseTimeMustBeLessThanMilliseconds(int maxMs)
        {
            var elapsedMs = (long)_scenarioContext["GenericElapsedMs"];
            Assert.Less(elapsedMs, maxMs, $"Expected response time < {maxMs}ms but got {elapsedMs}ms");
            TestContext.Progress.WriteLine($"Response time {elapsedMs}ms is within the {maxMs}ms threshold");
        }

        [When(@"I send a GET request to the Areas endpoint")]
        public async Task WhenISendAGETRequestToTheAreasEndpoint()
        {
            var (response, body, elapsedMs) = await AreasCrud.GetAreasWithStatusAsync();
            _scenarioContext["AreasResponse"] = response;
            _scenarioContext["AreasBody"] = body;
            _scenarioContext["AreasElapsedMs"] = elapsedMs;
            TestContext.Progress.WriteLine($"Areas API responded with status {(int)response.StatusCode} in {elapsedMs}ms");
        }

        [Then(@"the Areas response status code should be (\d+)")]
        public void ThenTheAreasResponseStatusCodeShouldBe(int expectedStatusCode)
        {
            var response = _scenarioContext["AreasResponse"] as HttpResponseMessage;
            Assert.IsNotNull(response, "Areas response was not captured");
            Assert.AreEqual(expectedStatusCode, (int)response.StatusCode, $"Expected status code {expectedStatusCode} but got {(int)response.StatusCode}");
        }

        [Then(@"the Areas response body Code should be (.*)")]
        public void ThenTheAreasResponseBodyCodeShouldBe(string expectedCode)
        {
            var body = _scenarioContext["AreasBody"] as AreasResponse;
            Assert.IsNotNull(body, "Areas response body was not captured or failed to deserialize");
            Assert.AreEqual(expectedCode, body.Code, $"Expected Code '{expectedCode}' but got '{body.Code}'");
        }

        [Then(@"the Areas response body Message should be empty")]
        public void ThenTheAreasResponseBodyMessageShouldBeEmpty()
        {
            var body = _scenarioContext["AreasBody"] as AreasResponse;
            Assert.IsNotNull(body, "Areas response body was not captured or failed to deserialize");
            Assert.IsTrue(string.IsNullOrEmpty(body.Message), $"Expected empty Message but got '{body.Message}'");
        }

        [Then(@"the Areas response Data array should exist")]
        public void ThenTheAreasResponseDataArrayShouldExist()
        {
            var body = _scenarioContext["AreasBody"] as AreasResponse;
            Assert.IsNotNull(body, "Areas response body was not captured or failed to deserialize");
            Assert.IsNotNull(body.Data, "Data array is null");
        }

        [Then(@"the Areas Data array length should be greater than (\d+)")]
        public void ThenTheAreasDataArrayLengthShouldBeGreaterThan(int minCount)
        {
            var body = _scenarioContext["AreasBody"] as AreasResponse;
            Assert.IsNotNull(body, "Areas response body was not captured or failed to deserialize");
            Assert.IsNotNull(body.Data, "Data array is null");
            Assert.Greater(body.Data.Count, minCount, $"Expected Data array length > {minCount} but got {body.Data.Count}");
        }

        [Given(@"the API returns a successful Areas response")]
        public async Task GivenTheAPIReturnsASuccessfulAreasResponse()
        {
            var (response, body, elapsedMs) = await AreasCrud.GetAreasWithStatusAsync();
            _scenarioContext["AreasResponse"] = response;
            _scenarioContext["AreasBody"] = body;
            _scenarioContext["AreasElapsedMs"] = elapsedMs;
            Assert.AreEqual(200, (int)response.StatusCode, "Expected successful response for Areas API");
            Assert.IsNotNull(body, "Areas response body was not captured");
            Assert.AreEqual("00", body.Code, "Expected Code '00' for successful response");
        }

        [Given(@"the API returns a list of areas")]
        public async Task GivenTheAPIReturnsAListOfAreas()
        {
            var (response, body, elapsedMs) = await AreasCrud.GetAreasWithStatusAsync();
            _scenarioContext["AreasResponse"] = response;
            _scenarioContext["AreasBody"] = body;
            _scenarioContext["AreasElapsedMs"] = elapsedMs;
            Assert.AreEqual(200, (int)response.StatusCode, "Expected successful response for Areas API");
            Assert.IsNotNull(body?.Data, "Areas Data array is null");
            Assert.Greater(body.Data.Count, 0, "Expected at least one area in the response");
        }

        [When(@"I inspect each item in the Areas Data array")]
        public void WhenIInspectEachItemInTheAreasDataArray()
        {
            var body = _scenarioContext["AreasBody"] as AreasResponse;
            Assert.IsNotNull(body?.Data, "Areas Data array is null or not captured");
            _scenarioContext["AreasDataItems"] = body.Data;
            TestContext.Progress.WriteLine($"Inspecting {body.Data.Count} area items");
        }

        [Then(@"each item in the Areas Data array should contain an areaid")]
        public void ThenEachItemInTheAreasDataArrayShouldContainAnAreaid()
        {
            var items = _scenarioContext["AreasBody"] as AreasResponse;
            Assert.IsNotNull(items?.Data, "Areas Data array is null");
            foreach (var item in items.Data)
            {
                Assert.IsNotNull(item.AreaId, $"Area item is missing areaid");
                Assert.IsNotEmpty(item.AreaId, $"Area item has empty areaid");
            }
        }

        [Then(@"each areaid should be a valid GUID")]
        public void ThenEachAreaidShouldBeAValidGUID()
        {
            var items = _scenarioContext["AreasBody"] as AreasResponse;
            Assert.IsNotNull(items?.Data, "Areas Data array is null");
            foreach (var item in items.Data)
            {
                Assert.IsTrue(Guid.TryParse(item.AreaId, out _), $"areaid '{item.AreaId}' is not a valid GUID");
            }
        }

        [Then(@"each item in the Areas Data array should contain an areaname")]
        public void ThenEachItemInTheAreasDataArrayShouldContainAnAreaname()
        {
            var items = _scenarioContext["AreasBody"] as AreasResponse;
            Assert.IsNotNull(items?.Data, "Areas Data array is null");
            foreach (var item in items.Data)
            {
                Assert.IsNotNull(item.AreaName, $"Area item with areaid '{item.AreaId}' is missing areaname");
            }
        }

        [Then(@"each areaname should be a non-empty string")]
        public void ThenEachAreanameShouldBeANonEmptyString()
        {
            var items = _scenarioContext["AreasBody"] as AreasResponse;
            Assert.IsNotNull(items?.Data, "Areas Data array is null");
            foreach (var item in items.Data)
            {
                Assert.IsNotEmpty(item.AreaName, $"Area item with areaid '{item.AreaId}' has empty areaname");
            }
        }

        [When(@"I collect all areaid values")]
        public void WhenICollectAllAreaidValues()
        {
            var body = _scenarioContext["AreasBody"] as AreasResponse;
            Assert.IsNotNull(body?.Data, "Areas Data array is null");
            var areaIds = body.Data.Select(a => a.AreaId).ToList();
            _scenarioContext["CollectedAreaIds"] = areaIds;
            TestContext.Progress.WriteLine($"Collected {areaIds.Count} areaid values");
        }

        [Then(@"there should be no duplicate areaid values")]
        public void ThenThereShouldBeNoDuplicateAreaidValues()
        {
            var areaIds = _scenarioContext["CollectedAreaIds"] as List<string>;
            Assert.IsNotNull(areaIds, "Area IDs were not collected");
            var duplicates = areaIds.GroupBy(id => id).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            Assert.IsEmpty(duplicates, $"Found duplicate areaid values: {string.Join(", ", duplicates)}");
        }

        [Then(@"the Areas API response time should be less than (\d+) milliseconds")]
        public void ThenTheAreasAPIResponseTimeShouldBeLessThanMilliseconds(int maxMs)
        {
            var elapsedMs = (long)_scenarioContext["AreasElapsedMs"];
            Assert.Less(elapsedMs, maxMs, $"Expected response time < {maxMs}ms but got {elapsedMs}ms");
        }

        [Given(@"I retrieve all valid area IDs from the Areas endpoint")]
        public async Task GivenIRetrieveAllValidAreaIDsFromTheAreasEndpoint()
        {
            var (response, body, elapsedMs) = await AreasCrud.GetAreasWithStatusAsync();
            Assert.AreEqual(200, (int)response.StatusCode, "Expected successful response from Areas API");
            Assert.IsNotNull(body?.Data, "Areas Data array is null");
            var validAreaIds = new HashSet<string>(body.Data.Select(a => a.AreaId));
            _scenarioContext["ValidAreaIds"] = validAreaIds;
            TestContext.Progress.WriteLine($"Retrieved {validAreaIds.Count} valid area IDs");
        }
    }
}
