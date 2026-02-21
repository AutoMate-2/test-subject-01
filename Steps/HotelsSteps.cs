using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using Reqnroll;
using ToIntegrations.CRUD.Hotels;
using to_integrations.HelperMethods;
using ToIntegrations.Models;

namespace ToIntegrations.Steps
{
    [Binding]
    public class HotelsSteps
    {
        private readonly ScenarioContext _scenarioContext;

        public HotelsSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [When(@"I send a GET request to the Hotels endpoint")]
        public async Task WhenISendAGETRequestToTheHotelsEndpoint()
        {
            var (response, body, elapsedMs) = await HotelsCrud.GetHotelsWithStatusAsync();
            _scenarioContext["HotelsResponse"] = response;
            _scenarioContext["HotelsBody"] = body;
            _scenarioContext["HotelsElapsedMs"] = elapsedMs;
            TestContext.Progress.WriteLine($"Hotels API responded with status {(int)response.StatusCode} in {elapsedMs}ms");
        }

        [Then(@"the Hotels response status code should be (\d+)")]
        public void ThenTheHotelsResponseStatusCodeShouldBe(int expectedStatusCode)
        {
            var response = _scenarioContext["HotelsResponse"] as HttpResponseMessage;
            Assert.IsNotNull(response, "Hotels response was not captured");
            Assert.AreEqual(expectedStatusCode, (int)response.StatusCode, $"Expected status code {expectedStatusCode} but got {(int)response.StatusCode}");
        }

        [Then(@"the Hotels response body Code should be (.*)")]
        public void ThenTheHotelsResponseBodyCodeShouldBe(string expectedCode)
        {
            var body = _scenarioContext["HotelsBody"] as HotelsResponse;
            Assert.IsNotNull(body, "Hotels response body was not captured or failed to deserialize");
            Assert.AreEqual(expectedCode, body.Code, $"Expected Code '{expectedCode}' but got '{body.Code}'");
        }

        [Then(@"the Hotels response body Message should be empty")]
        public void ThenTheHotelsResponseBodyMessageShouldBeEmpty()
        {
            var body = _scenarioContext["HotelsBody"] as HotelsResponse;
            Assert.IsNotNull(body, "Hotels response body was not captured or failed to deserialize");
            Assert.IsTrue(string.IsNullOrEmpty(body.Message), $"Expected empty Message but got '{body.Message}'");
        }

        [Then(@"the Hotels response Data array should exist")]
        public void ThenTheHotelsResponseDataArrayShouldExist()
        {
            var body = _scenarioContext["HotelsBody"] as HotelsResponse;
            Assert.IsNotNull(body, "Hotels response body was not captured or failed to deserialize");
            Assert.IsNotNull(body.Data, "Data array is null");
        }

        [Then(@"the Hotels Data array length should be greater than (\d+)")]
        public void ThenTheHotelsDataArrayLengthShouldBeGreaterThan(int minCount)
        {
            var body = _scenarioContext["HotelsBody"] as HotelsResponse;
            Assert.IsNotNull(body, "Hotels response body was not captured or failed to deserialize");
            Assert.IsNotNull(body.Data, "Data array is null");
            Assert.Greater(body.Data.Count, minCount, $"Expected Data array length > {minCount} but got {body.Data.Count}");
        }

        [Then(@"for each hotel in response (.*)")]
        public void ThenForEachHotelInResponse(string arrayName)
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
            _scenarioContext["HotelDataArrayItems"] = items;
            TestContext.Progress.WriteLine($"Found {items.Count} hotel items in '{arrayName}' array");
        }

        [Then(@"each hotel areaid should exist in the valid area IDs list")]
        public void ThenEachHotelAreaidShouldExistInTheValidAreaIDsList()
        {
            var validAreaIds = _scenarioContext["ValidAreaIds"] as HashSet<string>;
            Assert.IsNotNull(validAreaIds, "Valid area IDs were not captured");
            Assert.Greater(validAreaIds.Count, 0, "No valid area IDs were found");

            var items = _scenarioContext["HotelDataArrayItems"] as List<JsonElement>;
            Assert.IsNotNull(items, "Hotel data array items were not captured");

            var invalidHotels = new List<string>();
            int index = 0;
            foreach (var item in items)
            {
                if (item.TryGetProperty("areaid", out var areaIdValue))
                {
                    var areaId = areaIdValue.GetString();
                    if (!string.IsNullOrEmpty(areaId) && !validAreaIds.Contains(areaId))
                    {
                        string hotelId = "unknown";
                        if (item.TryGetProperty("hotelid", out var hotelIdValue))
                        {
                            hotelId = hotelIdValue.GetString() ?? "unknown";
                        }
                        string hotelName = "unknown";
                        if (item.TryGetProperty("hotelname", out var hotelNameValue))
                        {
                            hotelName = hotelNameValue.GetString() ?? "unknown";
                        }
                        invalidHotels.Add($"Hotel '{hotelName}' (ID: {hotelId}) has invalid areaid '{areaId}'");
                    }
                }
                index++;
            }

            Assert.IsEmpty(invalidHotels, $"Found hotels with invalid area references:\n{string.Join("\n", invalidHotels)}");
            TestContext.Progress.WriteLine($"All {items.Count} hotels have valid areaid references");
        }

        [Given(@"the API returns a successful Hotels response")]
        public async Task GivenTheAPIReturnsASuccessfulHotelsResponse()
        {
            var (response, body, elapsedMs) = await HotelsCrud.GetHotelsWithStatusAsync();
            _scenarioContext["HotelsResponse"] = response;
            _scenarioContext["HotelsBody"] = body;
            _scenarioContext["HotelsElapsedMs"] = elapsedMs;
            Assert.AreEqual(200, (int)response.StatusCode, "Expected successful response for Hotels API");
            Assert.IsNotNull(body, "Hotels response body was not captured");
            Assert.AreEqual("00", body.Code, "Expected Code '00' for successful response");
        }

        [Given(@"the API returns a list of hotels")]
        public async Task GivenTheAPIReturnsAListOfHotels()
        {
            var (response, body, elapsedMs) = await HotelsCrud.GetHotelsWithStatusAsync();
            _scenarioContext["HotelsResponse"] = response;
            _scenarioContext["HotelsBody"] = body;
            _scenarioContext["HotelsElapsedMs"] = elapsedMs;
            Assert.AreEqual(200, (int)response.StatusCode, "Expected successful response for Hotels API");
            Assert.IsNotNull(body?.Data, "Hotels Data array is null");
            Assert.Greater(body.Data.Count, 0, "Expected at least one hotel in the response");
        }

        [When(@"I inspect each item in the Hotels Data array")]
        public void WhenIInspectEachItemInTheHotelsDataArray()
        {
            var body = _scenarioContext["HotelsBody"] as HotelsResponse;
            Assert.IsNotNull(body?.Data, "Hotels Data array is null or not captured");
            _scenarioContext["HotelsDataItems"] = body.Data;
            TestContext.Progress.WriteLine($"Inspecting {body.Data.Count} hotel items");
        }

        [Then(@"each item in the Hotels Data array should contain a hotelid")]
        public void ThenEachItemInTheHotelsDataArrayShouldContainAHotelid()
        {
            var items = _scenarioContext["HotelsBody"] as HotelsResponse;
            Assert.IsNotNull(items?.Data, "Hotels Data array is null");
            foreach (var item in items.Data)
            {
                Assert.IsNotNull(item.HotelId, "Hotel item is missing hotelid");
                Assert.IsNotEmpty(item.HotelId, "Hotel item has empty hotelid");
            }
        }

        [Then(@"each hotelid should be a valid GUID")]
        public void ThenEachHotelidShouldBeAValidGUID()
        {
            var items = _scenarioContext["HotelsBody"] as HotelsResponse;
            Assert.IsNotNull(items?.Data, "Hotels Data array is null");
            foreach (var item in items.Data)
            {
                Assert.IsTrue(Guid.TryParse(item.HotelId, out _), $"hotelid '{item.HotelId}' is not a valid GUID");
            }
        }

        [Then(@"each item in the Hotels Data array should contain a hotelname")]
        public void ThenEachItemInTheHotelsDataArrayShouldContainAHotelname()
        {
            var items = _scenarioContext["HotelsBody"] as HotelsResponse;
            Assert.IsNotNull(items?.Data, "Hotels Data array is null");
            foreach (var item in items.Data)
            {
                Assert.IsNotNull(item.HotelName, $"Hotel item with hotelid '{item.HotelId}' is missing hotelname");
            }
        }

        [Then(@"each hotelname should be a non-empty string")]
        public void ThenEachHotelnameShouldBeANonEmptyString()
        {
            var items = _scenarioContext["HotelsBody"] as HotelsResponse;
            Assert.IsNotNull(items?.Data, "Hotels Data array is null");
            foreach (var item in items.Data)
            {
                Assert.IsNotEmpty(item.HotelName, $"Hotel item with hotelid '{item.HotelId}' has empty hotelname");
            }
        }

        [When(@"I collect all hotelid values")]
        public void WhenICollectAllHotelidValues()
        {
            var body = _scenarioContext["HotelsBody"] as HotelsResponse;
            Assert.IsNotNull(body?.Data, "Hotels Data array is null");
            var hotelIds = body.Data.Select(h => h.HotelId).ToList();
            _scenarioContext["CollectedHotelIds"] = hotelIds;
            TestContext.Progress.WriteLine($"Collected {hotelIds.Count} hotelid values");
        }

        [Then(@"there should be no duplicate hotelid values")]
        public void ThenThereShouldBeNoDuplicateHotelidValues()
        {
            var hotelIds = _scenarioContext["CollectedHotelIds"] as List<string>;
            Assert.IsNotNull(hotelIds, "Hotel IDs were not collected");
            var duplicates = hotelIds.GroupBy(id => id).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            Assert.IsEmpty(duplicates, $"Found duplicate hotelid values: {string.Join(", ", duplicates)}");
        }

        [Then(@"the Hotels API response time should be less than (\d+) milliseconds")]
        public void ThenTheHotelsAPIResponseTimeShouldBeLessThanMilliseconds(int maxMs)
        {
            var elapsedMs = (long)_scenarioContext["HotelsElapsedMs"];
            Assert.Less(elapsedMs, maxMs, $"Expected response time < {maxMs}ms but got {elapsedMs}ms");
        }

        [Given(@"I retrieve all valid hotel IDs from the Hotels endpoint")]
        public async Task GivenIRetrieveAllValidHotelIDsFromTheHotelsEndpoint()
        {
            var (response, body, elapsedMs) = await HotelsCrud.GetHotelsWithStatusAsync();
            Assert.AreEqual(200, (int)response.StatusCode, "Expected successful response from Hotels API");
            Assert.IsNotNull(body?.Data, "Hotels Data array is null");
            var validHotelIds = new HashSet<string>(body.Data.Select(h => h.HotelId));
            _scenarioContext["ValidHotelIds"] = validHotelIds;
            TestContext.Progress.WriteLine($"Retrieved {validHotelIds.Count} valid hotel IDs");
        }
    }
}
