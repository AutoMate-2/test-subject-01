using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using TechTalk.SpecFlow;
using to_integrations.CRUD.Cities;
using to_integrations.HelperMethods;
using to_integrations.Models;

namespace to_integrations.Steps
{
    [Binding]
    public class CitiesSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private CitiesResponse _response;
        private HttpStatusCode _statusCode;
        private long _responseTimeMs;
        private List<string> _collectedCityIds;

        public CitiesSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"I have valid authentication credentials")]
        public void GivenIHaveValidAuthenticationCredentials()
        {
            var token = TokenCache.CachedToken;
            Assert.IsNotNull(token, "Authentication token should be available");
            Assert.IsFalse(string.IsNullOrEmpty(token), "Authentication token should not be empty");
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
        }

        [When(@"I send a GET request to the Cities endpoint")]
        public async Task WhenISendAGETRequestToTheCitiesEndpoint()
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
            TestContext.Progress.WriteLine($"Cities endpoint responded in {_responseTimeMs}ms with status code {(int)_statusCode}");
        }

        [When(@"I inspect each item in the Data array")]
        public void WhenIInspectEachItemInTheDataArray()
        {
            if (_response == null && _scenarioContext.ContainsKey("CitiesResponse"))
            {
                _response = _scenarioContext["CitiesResponse"] as CitiesResponse;
            }
            
            Assert.IsNotNull(_response, "Response should not be null");
            Assert.IsNotNull(_response.Data, "Response Data array should exist");
            Assert.Greater(_response.Data.Count, 0, "Data array should contain at least one item to inspect");
            TestContext.Progress.WriteLine($"Inspecting {_response.Data.Count} items in the Data array");
        }

        [When(@"I collect all cityid values")]
        public void WhenICollectAllCityidValues()
        {
            if (_response == null && _scenarioContext.ContainsKey("CitiesResponse"))
            {
                _response = _scenarioContext["CitiesResponse"] as CitiesResponse;
            }
            
            Assert.IsNotNull(_response, "Response should not be null");
            Assert.IsNotNull(_response.Data, "Response Data array should exist");
            
            _collectedCityIds = _response.Data.Select(c => c.CityId).ToList();
            _scenarioContext["CollectedCityIds"] = _collectedCityIds;
            TestContext.Progress.WriteLine($"Collected {_collectedCityIds.Count} cityid values");
        }

        [Then(@"the response status code should be (\d+)")]
        public void ThenTheResponseStatusCodeShouldBe(int expectedStatusCode)
        {
            if (_scenarioContext.ContainsKey("StatusCode"))
            {
                _statusCode = (HttpStatusCode)_scenarioContext["StatusCode"];
            }
            
            int actualStatusCode = (int)_statusCode;
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Expected status code {expectedStatusCode} but got {actualStatusCode}");
            TestContext.Progress.WriteLine($"Response status code is {actualStatusCode} as expected");
        }

        [Then(@"the response body Code should be \"(.*)\"")]
        public void ThenTheResponseBodyCodeShouldBe(string expectedCode)
        {
            if (_response == null && _scenarioContext.ContainsKey("CitiesResponse"))
            {
                _response = _scenarioContext["CitiesResponse"] as CitiesResponse;
            }
            
            Assert.IsNotNull(_response, "Response should not be null");
            Assert.AreEqual(expectedCode, _response.Code, $"Response Code should be '{expectedCode}' but was '{_response.Code}'");
            TestContext.Progress.WriteLine($"Response body Code is '{_response.Code}' as expected");
        }

        [Then(@"the response body Message should be empty")]
        public void ThenTheResponseBodyMessageShouldBeEmpty()
        {
            if (_response == null && _scenarioContext.ContainsKey("CitiesResponse"))
            {
                _response = _scenarioContext["CitiesResponse"] as CitiesResponse;
            }
            
            Assert.IsNotNull(_response, "Response should not be null");
            Assert.IsTrue(string.IsNullOrEmpty(_response.Message), $"Response Message should be empty but was '{_response.Message}'");
            TestContext.Progress.WriteLine("Response body Message is empty as expected");
        }

        [Then(@"the response Data array should exist")]
        public void ThenTheResponseDataArrayShouldExist()
        {
            if (_response == null && _scenarioContext.ContainsKey("CitiesResponse"))
            {
                _response = _scenarioContext["CitiesResponse"] as CitiesResponse;
            }
            
            Assert.IsNotNull(_response, "Response should not be null");
            Assert.IsNotNull(_response.Data, "Response Data array should exist");
            TestContext.Progress.WriteLine($"Data array exists with {_response.Data.Count} items");
        }

        [Then(@"the Data array length should be greater than (\d+)")]
        public void ThenTheDataArrayLengthShouldBeGreaterThan(int minLength)
        {
            if (_response == null && _scenarioContext.ContainsKey("CitiesResponse"))
            {
                _response = _scenarioContext["CitiesResponse"] as CitiesResponse;
            }
            
            Assert.IsNotNull(_response, "Response should not be null");
            Assert.IsNotNull(_response.Data, "Response Data array should exist");
            Assert.Greater(_response.Data.Count, minLength, $"Data array length should be greater than {minLength}");
            TestContext.Progress.WriteLine($"Data array contains {_response.Data.Count} items (expected > {minLength})");
        }

        [Then(@"each item in the Data array should contain a cityid")]
        public void ThenEachItemInTheDataArrayShouldContainACityid()
        {
            if (_response == null && _scenarioContext.ContainsKey("CitiesResponse"))
            {
                _response = _scenarioContext["CitiesResponse"] as CitiesResponse;
            }
            
            Assert.IsNotNull(_response?.Data, "Response Data should not be null");
            foreach (var city in _response.Data)
            {
                Assert.IsNotNull(city.CityId, "Each city should have a cityid");
            }
        }

        [Then(@"each item should contain a cityid")]
        public void ThenEachItemShouldContainACityid()
        {
            if (_response == null && _scenarioContext.ContainsKey("CitiesResponse"))
            {
                _response = _scenarioContext["CitiesResponse"] as CitiesResponse;
            }
            
            Assert.IsNotNull(_response?.Data, "Response Data should not be null");
            int itemIndex = 0;
            foreach (var city in _response.Data)
            {
                Assert.IsNotNull(city.CityId, $"City at index {itemIndex} should have a cityid");
                itemIndex++;
            }
            TestContext.Progress.WriteLine($"All {_response.Data.Count} items contain a cityid");
        }

        [Then(@"each cityid should be a valid GUID")]
        public void ThenEachCityidShouldBeAValidGUID()
        {
            if (_response == null && _scenarioContext.ContainsKey("CitiesResponse"))
            {
                _response = _scenarioContext["CitiesResponse"] as CitiesResponse;
            }
            
            Assert.IsNotNull(_response?.Data, "Response Data should not be null");
            int validCount = 0;
            foreach (var city in _response.Data)
            {
                Assert.IsTrue(Guid.TryParse(city.CityId, out _), $"CityId '{city.CityId}' should be a valid GUID");
                validCount++;
            }
            TestContext.Progress.WriteLine($"All {validCount} cityid values are valid GUIDs");
        }

        [Then(@"each item should contain a cityname")]
        public void ThenEachItemShouldContainACityname()
        {
            if (_response == null && _scenarioContext.ContainsKey("CitiesResponse"))
            {
                _response = _scenarioContext["CitiesResponse"] as CitiesResponse;
            }
            
            Assert.IsNotNull(_response?.Data, "Response Data should not be null");
            int itemIndex = 0;
            foreach (var city in _response.Data)
            {
                Assert.IsNotNull(city.CityName, $"City at index {itemIndex} (id: {city.CityId}) should have a cityname");
                itemIndex++;
            }
            TestContext.Progress.WriteLine($"All {_response.Data.Count} items contain a cityname");
        }

        [Then(@"each cityname should be a non-empty string")]
        public void ThenEachCitynameShouldBeANonEmptyString()
        {
            if (_response == null && _scenarioContext.ContainsKey("CitiesResponse"))
            {
                _response = _scenarioContext["CitiesResponse"] as CitiesResponse;
            }
            
            Assert.IsNotNull(_response?.Data, "Response Data should not be null");
            int validCount = 0;
            foreach (var city in _response.Data)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(city.CityName), $"CityName should not be empty for city {city.CityId}");
                validCount++;
            }
            TestContext.Progress.WriteLine($"All {validCount} cityname values are non-empty strings");
        }

        [Then(@"there should be no duplicate cityid values")]
        public void ThenThereShouldBeNoDuplicateCityidValues()
        {
            List<string> cityIds;
            
            if (_collectedCityIds != null)
            {
                cityIds = _collectedCityIds;
            }
            else if (_scenarioContext.ContainsKey("CollectedCityIds"))
            {
                cityIds = _scenarioContext["CollectedCityIds"] as List<string>;
            }
            else if (_response?.Data != null)
            {
                cityIds = _response.Data.Select(c => c.CityId).ToList();
            }
            else if (_scenarioContext.ContainsKey("CitiesResponse"))
            {
                var response = _scenarioContext["CitiesResponse"] as CitiesResponse;
                Assert.IsNotNull(response?.Data, "Response Data should not be null");
                cityIds = response.Data.Select(c => c.CityId).ToList();
            }
            else
            {
                Assert.Fail("No city data available to check for duplicates");
                return;
            }
            
            var distinctCityIds = cityIds.Distinct().ToList();
            var duplicates = cityIds.GroupBy(x => x)
                                    .Where(g => g.Count() > 1)
                                    .Select(g => g.Key)
                                    .ToList();
            
            if (duplicates.Any())
            {
                TestContext.Progress.WriteLine($"Found duplicate cityid values: {string.Join(", ", duplicates)}");
            }
            
            Assert.AreEqual(distinctCityIds.Count, cityIds.Count, 
                $"There should be no duplicate cityid values. Found {cityIds.Count - distinctCityIds.Count} duplicates: {string.Join(", ", duplicates)}");
            TestContext.Progress.WriteLine($"Verified {cityIds.Count} cityid values are unique - no duplicates found");
        }

        [Then(@"the API response time should be less than (\d+) milliseconds")]
        public void ThenTheAPIResponseTimeShouldBeLessThanMilliseconds(int maxMilliseconds)
        {
            if (_scenarioContext.ContainsKey("ResponseTimeMs"))
            {
                _responseTimeMs = (long)_scenarioContext["ResponseTimeMs"];
            }
            
            Assert.Less(_responseTimeMs, maxMilliseconds, $"API response time ({_responseTimeMs}ms) should be less than {maxMilliseconds}ms");
            TestContext.Progress.WriteLine($"Response time {_responseTimeMs}ms is within acceptable limit of {maxMilliseconds}ms");
        }
    }
}
