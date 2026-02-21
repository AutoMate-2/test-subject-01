using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using Reqnroll;
using to_integrations.CRUD.Cities;
using to_integrations.HelperMethods;
using to_integrations.Models;

namespace ToIntegrations.Steps
{
    [Binding]
    public class CitiesSteps
    {
        private readonly ScenarioContext _scenarioContext;

        public CitiesSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [When(@"I send a GET request to the Cities endpoint")]
        public async Task WhenISendAGETRequestToTheCitiesEndpoint()
        {
            var (response, body, elapsedMs) = await CitiesCrud.GetCitiesWithStatusAsync();
            _scenarioContext["CitiesResponse"] = response;
            _scenarioContext["CitiesBody"] = body;
            _scenarioContext["CitiesElapsedMs"] = elapsedMs;
            TestContext.Progress.WriteLine($"Cities API responded with status {(int)response.StatusCode} in {elapsedMs}ms");
        }

        [Then(@"the response status code should be (\d+)")]
        public void ThenTheResponseStatusCodeShouldBe(int expectedStatusCode)
        {
            var response = _scenarioContext["CitiesResponse"] as HttpResponseMessage;
            Assert.IsNotNull(response, "Cities response was not captured");
            Assert.AreEqual(expectedStatusCode, (int)response.StatusCode, $"Expected status code {expectedStatusCode} but got {(int)response.StatusCode}");
        }

        [Then(@"the response body Code should be (.*)")]
        public void ThenTheResponseBodyCodeShouldBe(string expectedCode)
        {
            var body = _scenarioContext["CitiesBody"] as CitiesResponse;
            Assert.IsNotNull(body, "Cities response body was not captured or failed to deserialize");
            Assert.AreEqual(expectedCode, body.Code, $"Expected Code '{expectedCode}' but got '{body.Code}'");
        }

        [Then(@"the response body Message should be empty")]
        public void ThenTheResponseBodyMessageShouldBeEmpty()
        {
            var body = _scenarioContext["CitiesBody"] as CitiesResponse;
            Assert.IsNotNull(body, "Cities response body was not captured or failed to deserialize");
            Assert.IsTrue(string.IsNullOrEmpty(body.Message), $"Expected empty Message but got '{body.Message}'");
        }

        [Then(@"the response Data array should exist")]
        public void ThenTheResponseDataArrayShouldExist()
        {
            var body = _scenarioContext["CitiesBody"] as CitiesResponse;
            Assert.IsNotNull(body, "Cities response body was not captured or failed to deserialize");
            Assert.IsNotNull(body.Data, "Data array is null");
        }

        [Then(@"the Data array length should be greater than (\d+)")]
        public void ThenTheDataArrayLengthShouldBeGreaterThan(int minCount)
        {
            var body = _scenarioContext["CitiesBody"] as CitiesResponse;
            Assert.IsNotNull(body, "Cities response body was not captured or failed to deserialize");
            Assert.IsNotNull(body.Data, "Data array is null");
            Assert.Greater(body.Data.Count, minCount, $"Expected Data array length > {minCount} but got {body.Data.Count}");
        }

        [Given(@"the API returns a successful response")]
        public async Task GivenTheAPIReturnsASuccessfulResponse()
        {
            var (response, body, elapsedMs) = await CitiesCrud.GetCitiesWithStatusAsync();
            _scenarioContext["CitiesResponse"] = response;
            _scenarioContext["CitiesBody"] = body;
            _scenarioContext["CitiesElapsedMs"] = elapsedMs;
            Assert.AreEqual(200, (int)response.StatusCode, "Expected successful response for Cities API");
            Assert.IsNotNull(body, "Cities response body was not captured");
            Assert.AreEqual("00", body.Code, "Expected Code '00' for successful response");
        }

        [When(@"I inspect each item in the Data array")]
        public void WhenIInspectEachItemInTheDataArray()
        {
            var body = _scenarioContext["CitiesBody"] as CitiesResponse;
            Assert.IsNotNull(body?.Data, "Cities Data array is null or not captured");
            _scenarioContext["CitiesDataItems"] = body.Data;
            TestContext.Progress.WriteLine($"Inspecting {body.Data.Count} city items");
        }

        [Then(@"each city item should contain a cityid")]
        public void ThenEachCityItemShouldContainACityid()
        {
            var items = _scenarioContext["CitiesBody"] as CitiesResponse;
            Assert.IsNotNull(items?.Data, "Cities Data array is null");
            foreach (var item in items.Data)
            {
                Assert.IsNotNull(item.CityId, "City item is missing cityid");
                Assert.IsNotEmpty(item.CityId, "City item has empty cityid");
            }
        }

        [Then(@"each item in the Data array should contain a cityid")]
        public void ThenEachItemInTheDataArrayShouldContainACityid()
        {
            ThenEachCityItemShouldContainACityid();
        }

        [Then(@"each cityid should be a valid GUID")]
        public void ThenEachCityidShouldBeAValidGUID()
        {
            var items = _scenarioContext["CitiesBody"] as CitiesResponse;
            Assert.IsNotNull(items?.Data, "Cities Data array is null");
            foreach (var item in items.Data)
            {
                Assert.IsTrue(Guid.TryParse(item.CityId, out _), $"cityid '{item.CityId}' is not a valid GUID");
            }
        }

        [Then(@"each item should contain a cityname")]
        public void ThenEachItemShouldContainACityname()
        {
            var items = _scenarioContext["CitiesBody"] as CitiesResponse;
            Assert.IsNotNull(items?.Data, "Cities Data array is null");
            foreach (var item in items.Data)
            {
                Assert.IsNotNull(item.CityName, $"City item with cityid '{item.CityId}' is missing cityname");
            }
        }

        [Then(@"each cityname should be a non-empty string")]
        public void ThenEachCitynameShouldBeANonEmptyString()
        {
            var items = _scenarioContext["CitiesBody"] as CitiesResponse;
            Assert.IsNotNull(items?.Data, "Cities Data array is null");
            foreach (var item in items.Data)
            {
                Assert.IsNotEmpty(item.CityName, $"City item with cityid '{item.CityId}' has empty cityname");
            }
        }

        [Given(@"the API returns a list of cities")]
        public async Task GivenTheAPIReturnsAListOfCities()
        {
            var (response, body, elapsedMs) = await CitiesCrud.GetCitiesWithStatusAsync();
            _scenarioContext["CitiesResponse"] = response;
            _scenarioContext["CitiesBody"] = body;
            _scenarioContext["CitiesElapsedMs"] = elapsedMs;
            Assert.AreEqual(200, (int)response.StatusCode, "Expected successful response for Cities API");
            Assert.IsNotNull(body?.Data, "Cities Data array is null");
            Assert.Greater(body.Data.Count, 0, "Expected at least one city in the response");
        }

        [When(@"I collect all cityid values")]
        public void WhenICollectAllCityidValues()
        {
            var body = _scenarioContext["CitiesBody"] as CitiesResponse;
            Assert.IsNotNull(body?.Data, "Cities Data array is null");
            var cityIds = body.Data.Select(c => c.CityId).ToList();
            _scenarioContext["CollectedCityIds"] = cityIds;
            TestContext.Progress.WriteLine($"Collected {cityIds.Count} cityid values");
        }

        [Then(@"there should be no duplicate cityid values")]
        public void ThenThereShouldBeNoDuplicateCityidValues()
        {
            var cityIds = _scenarioContext["CollectedCityIds"] as List<string>;
            Assert.IsNotNull(cityIds, "City IDs were not collected");
            var duplicates = cityIds.GroupBy(id => id).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            Assert.IsEmpty(duplicates, $"Found duplicate cityid values: {string.Join(", ", duplicates)}");
        }

        [Then(@"the API response time should be less than (\d+) milliseconds")]
        public void ThenTheAPIResponseTimeShouldBeLessThanMilliseconds(int maxMs)
        {
            var elapsedMs = (long)_scenarioContext["CitiesElapsedMs"];
            Assert.Less(elapsedMs, maxMs, $"Expected response time < {maxMs}ms but got {elapsedMs}ms");
        }

        // Removed duplicate GivenIStoreAllValuesAsValidAreaIds - moved to CommonSteps

        [Then(@"for each city in response (.*)")]
        public void ThenForEachCityInResponse(string arrayName)
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
            _scenarioContext["CityDataArrayItems"] = items;
            TestContext.Progress.WriteLine($"Found {items.Count} city items in '{arrayName}' array");
        }

        [Then(@"each city areaid should exist in the valid area IDs list")]
        public void ThenEachCityAreaidShouldExistInTheValidAreaIDsList()
        {
            var validAreaIds = _scenarioContext["ValidAreaIds"] as HashSet<string>;
            Assert.IsNotNull(validAreaIds, "Valid area IDs were not captured");
            Assert.Greater(validAreaIds.Count, 0, "No valid area IDs were found");

            var items = _scenarioContext["CityDataArrayItems"] as List<JsonElement>;
            Assert.IsNotNull(items, "City data array items were not captured");

            var invalidCities = new List<string>();
            int index = 0;
            foreach (var item in items)
            {
                if (item.TryGetProperty("areaid", out var areaIdValue))
                {
                    var areaId = areaIdValue.GetString();
                    if (!string.IsNullOrEmpty(areaId) && !validAreaIds.Contains(areaId))
                    {
                        string cityId = "unknown";
                        if (item.TryGetProperty("cityid", out var cityIdValue))
                        {
                            cityId = cityIdValue.GetString() ?? "unknown";
                        }
                        invalidCities.Add($"City '{cityId}' has invalid areaid '{areaId}'");
                    }
                }
                index++;
            }

            Assert.IsEmpty(invalidCities, $"Found cities with invalid area references:\n{string.Join("\n", invalidCities)}");
            TestContext.Progress.WriteLine($"All {items.Count} cities have valid areaid references");
        }
    }
}
