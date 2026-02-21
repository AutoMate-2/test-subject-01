using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using Reqnroll;
using to_integrations.CRUD.Cities;
using to_integrations.HelperMethods;
using to_integrations.Models;

namespace to_integrations.Steps
{
    [Binding]
    public class CitiesSteps
    {
        private readonly ScenarioContext _scenarioContext;

        public CitiesSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"I have valid authentication credentials")]
        public void GivenIHaveValidAuthenticationCredentials()
        {
<<<<<<< HEAD
            var agentId = AppConfig.GetValue("AgentId");
            var agentPassword = AppConfig.GetValue("AgentPassword");
            Assert.IsNotNull(agentId, "AgentId should be configured");
            Assert.IsFalse(string.IsNullOrEmpty(agentId), "AgentId should not be empty");
            Assert.IsNotNull(agentPassword, "AgentPassword should be configured");
            Assert.IsFalse(string.IsNullOrEmpty(agentPassword), "AgentPassword should not be empty");
            TestContext.Progress.WriteLine("Valid authentication credentials confirmed");
        }

        [Given(@"the API returns a successful response")]
        public async Task GivenTheAPIReturnsASuccessfulResponse()
        {
            var citiesCrud = new CitiesCrud();
            var stopwatch = Stopwatch.StartNew();
            var result = await citiesCrud.GetCitiesWithStatusAsync();
            stopwatch.Stop();
            _response = result.Response;
            _statusCode = result.StatusCode;
            _responseTimeMs = stopwatch.ElapsedMilliseconds;
            _scenarioContext["CitiesResponse"] = _response;
            _scenarioContext["ResponseTimeMs"] = _responseTimeMs;
            _scenarioContext["StatusCode"] = _statusCode;
            
            Assert.IsNotNull(_response, "API response should not be null");
            Assert.IsNotNull(_response.Data, "API response Data should not be null");
            TestContext.Progress.WriteLine($"Cities endpoint responded successfully in {_responseTimeMs}ms with {_response.Data.Count} items");
        }

        [Given(@"the API returns a list of cities")]
        public async Task GivenTheAPIReturnsAListOfCities()
        {
            var citiesCrud = new CitiesCrud();
            var stopwatch = Stopwatch.StartNew();
            var result = await citiesCrud.GetCitiesWithStatusAsync();
            stopwatch.Stop();
            _response = result.Response;
            _statusCode = result.StatusCode;
            _responseTimeMs = stopwatch.ElapsedMilliseconds;
            _scenarioContext["CitiesResponse"] = _response;
            _scenarioContext["ResponseTimeMs"] = _responseTimeMs;
            _scenarioContext["StatusCode"] = _statusCode;
            
            Assert.IsNotNull(_response, "API response should not be null");
            Assert.IsNotNull(_response.Data, "API response Data should not be null");
            Assert.Greater(_response.Data.Count, 0, "API should return at least one city");
            TestContext.Progress.WriteLine($"Cities endpoint returned {_response.Data.Count} cities in {_responseTimeMs}ms");
=======
            Assert.IsNotNull(TokenCache.CachedToken, "Authentication token should be cached");
            TestContext.Progress.WriteLine("Valid authentication credentials confirmed.");
>>>>>>> fe59efb2efbd46ad04271a7b350e0108db3311ef
        }

        [When(@"I send a GET request to the Cities endpoint")]
        public async Task WhenISendAGETRequestToTheCitiesEndpoint()
        {
            var (response, body, elapsedMs) = await CitiesCrud.GetCitiesWithStatusAsync();
            _scenarioContext["CitiesHttpResponse"] = response;
            _scenarioContext["CitiesResponseBody"] = body;
            _scenarioContext["CitiesElapsedMs"] = elapsedMs;
            TestContext.Progress.WriteLine($"Cities API responded in {elapsedMs}ms with status {(int)response.StatusCode}");
        }

        [Then(@"the response status code should be (\d+)")]
        public void ThenTheResponseStatusCodeShouldBe(int expectedStatusCode)
        {
            var response = (HttpResponseMessage)_scenarioContext["CitiesHttpResponse"];
            Assert.AreEqual(expectedStatusCode, (int)response.StatusCode, $"Expected status code {expectedStatusCode} but got {(int)response.StatusCode}");
        }

<<<<<<< HEAD
        [Then(@"the response body Code should be ""(.*)""")]
=======
<<<<<<< HEAD
        [Then(@"the response body Code should be ""(.*)""")]
=======
        [Then(@"the response body Code should be ""(.*)""$")]
>>>>>>> ca0b864a037c063f4f0ffcb95fc6b5dcb30b07f2
>>>>>>> fe59efb2efbd46ad04271a7b350e0108db3311ef
        public void ThenTheResponseBodyCodeShouldBe(string expectedCode)
        {
            var body = (CitiesResponse)_scenarioContext["CitiesResponseBody"];
            Assert.IsNotNull(body, "Response body should not be null");
            Assert.AreEqual(expectedCode, body.Code, $"Expected Code '{expectedCode}' but got '{body.Code}'");
        }

        [Then(@"the response body Message should be empty")]
        public void ThenTheResponseBodyMessageShouldBeEmpty()
        {
            var body = (CitiesResponse)_scenarioContext["CitiesResponseBody"];
            Assert.IsTrue(string.IsNullOrEmpty(body.Message), $"Expected empty Message but got '{body.Message}'");
        }

        [Then(@"the response Data array should exist")]
        public void ThenTheResponseDataArrayShouldExist()
        {
            var body = (CitiesResponse)_scenarioContext["CitiesResponseBody"];
            Assert.IsNotNull(body.Data, "Data array should exist in the response");
        }

        [Then(@"the Data array length should be greater than (\d+)")]
        public void ThenTheDataArrayLengthShouldBeGreaterThan(int minCount)
        {
            var body = (CitiesResponse)_scenarioContext["CitiesResponseBody"];
            Assert.Greater(body.Data.Count, minCount, $"Expected Data array length greater than {minCount} but got {body.Data.Count}");
        }

        [Given(@"the API returns a successful response")]
        public async Task GivenTheAPIReturnsASuccessfulResponse()
        {
            var (response, body, elapsedMs) = await CitiesCrud.GetCitiesWithStatusAsync();
            _scenarioContext["CitiesHttpResponse"] = response;
            _scenarioContext["CitiesResponseBody"] = body;
            _scenarioContext["CitiesElapsedMs"] = elapsedMs;
            Assert.AreEqual(200, (int)response.StatusCode, "Expected 200 OK from Cities API");
        }

        [When(@"I inspect each item in the Data array")]
        public void WhenIInspectEachItemInTheDataArray()
        {
            var body = (CitiesResponse)_scenarioContext["CitiesResponseBody"];
            Assert.IsNotNull(body.Data, "Data array should exist");
            Assert.Greater(body.Data.Count, 0, "Data array should not be empty");
            TestContext.Progress.WriteLine($"Inspecting {body.Data.Count} city items.");
        }

        [Then(@"each item should contain a cityid")]
        public void ThenEachItemShouldContainACityid()
        {
            var body = (CitiesResponse)_scenarioContext["CitiesResponseBody"];
            foreach (var city in body.Data)
            {
                Assert.IsNotNull(city.CityId, "Each city item should contain a cityid");
                Assert.IsNotEmpty(city.CityId, "cityid should not be empty");
            }
        }

        [Then(@"each item in the Data array should contain a cityid")]
        public void ThenEachItemInTheDataArrayShouldContainACityid()
        {
            ThenEachItemShouldContainACityid();
        }

        [Then(@"each cityid should be a valid GUID")]
        public void ThenEachCityidShouldBeAValidGUID()
        {
            var body = (CitiesResponse)_scenarioContext["CitiesResponseBody"];
            foreach (var city in body.Data)
            {
                Assert.IsTrue(Guid.TryParse(city.CityId, out _), $"cityid '{city.CityId}' is not a valid GUID");
            }
        }

        [Then(@"each item should contain a cityname")]
        public void ThenEachItemShouldContainACityname()
        {
            var body = (CitiesResponse)_scenarioContext["CitiesResponseBody"];
            foreach (var city in body.Data)
            {
                Assert.IsNotNull(city.CityName, "Each city item should contain a cityname");
            }
        }

        [Then(@"each cityname should be a non-empty string")]
        public void ThenEachCitynameShouldBeANonEmptyString()
        {
            var body = (CitiesResponse)_scenarioContext["CitiesResponseBody"];
            foreach (var city in body.Data)
            {
                Assert.IsNotEmpty(city.CityName, "cityname should be a non-empty string");
            }
        }

        [Given(@"the API returns a list of cities")]
        public async Task GivenTheAPIReturnsAListOfCities()
        {
            var (response, body, elapsedMs) = await CitiesCrud.GetCitiesWithStatusAsync();
            _scenarioContext["CitiesHttpResponse"] = response;
            _scenarioContext["CitiesResponseBody"] = body;
            _scenarioContext["CitiesElapsedMs"] = elapsedMs;
            Assert.IsNotNull(body.Data, "Data array should exist");
            Assert.Greater(body.Data.Count, 0, "Cities list should not be empty");
        }

        [When(@"I collect all cityid values")]
        public void WhenICollectAllCityidValues()
        {
            var body = (CitiesResponse)_scenarioContext["CitiesResponseBody"];
            var cityIds = body.Data.Select(c => c.CityId).ToList();
            _scenarioContext["CityIds"] = cityIds;
            TestContext.Progress.WriteLine($"Collected {cityIds.Count} cityid values.");
        }

        [Then(@"there should be no duplicate cityid values")]
        public void ThenThereShouldBeNoDuplicateCityidValues()
        {
            var cityIds = (List<string>)_scenarioContext["CityIds"];
            var duplicates = cityIds.GroupBy(id => id).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            Assert.IsEmpty(duplicates, $"Found duplicate cityid values: {string.Join(", ", duplicates)}");
        }

        [Then(@"the API response time should be less than (\d+) milliseconds")]
        public void ThenTheAPIResponseTimeShouldBeLessThanMilliseconds(int maxMs)
        {
            var elapsedMs = (long)_scenarioContext["CitiesElapsedMs"];
            Assert.Less(elapsedMs, maxMs, $"API response time {elapsedMs}ms exceeded {maxMs}ms threshold");
        }
    }
}
