using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
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
        private HttpResponseMessage _httpResponse;
        private long _responseTimeMs;
        private string _agentId;
        private string _agentPassword;

        public CitiesSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"I have valid authentication credentials")]
        public void GivenIHaveValidAuthenticationCredentials()
        {
            _agentId = CitiesPresetup.ValidAgentId;
            _agentPassword = CitiesPresetup.ValidAgentPassword;
            TestContext.Progress.WriteLine($"Using agent credentials for authentication");
        }

        [When(@"I send a GET request to the Cities endpoint")]
        public async Task WhenISendAGETRequestToTheCitiesEndpoint()
        {
            var citiesCrud = new CitiesCrud();
            var stopwatch = Stopwatch.StartNew();
            
            _httpResponse = await citiesCrud.GetCitiesAsync(_agentId, _agentPassword);
            
            stopwatch.Stop();
            _responseTimeMs = stopwatch.ElapsedMilliseconds;
            
            var content = await _httpResponse.Content.ReadAsStringAsync();
            TestContext.Progress.WriteLine($"Response received in {_responseTimeMs}ms");
            TestContext.Progress.WriteLine($"Response content: {content}");
            
            if (_httpResponse.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                _response = JsonSerializer.Deserialize<CitiesResponse>(content, options);
            }
            
            _scenarioContext["HttpResponse"] = _httpResponse;
            _scenarioContext["CitiesResponse"] = _response;
            _scenarioContext["ResponseTimeMs"] = _responseTimeMs;
        }

        [Then(@"the response status code should be (\d+)")]
        public void ThenTheResponseStatusCodeShouldBe(int expectedStatusCode)
        {
            Assert.IsNotNull(_httpResponse, "HTTP response should not be null");
            Assert.AreEqual(expectedStatusCode, (int)_httpResponse.StatusCode, 
                $"Expected status code {expectedStatusCode} but got {(int)_httpResponse.StatusCode}");
        }

        [Then(@"the response body Code should be ""(.*)""")
        public void ThenTheResponseBodyCodeShouldBe(string expectedCode)
        {
            Assert.IsNotNull(_response, "Response should not be null");
            Assert.AreEqual(expectedCode, _response.Code, 
                $"Expected Code '{expectedCode}' but got '{_response.Code}'");
        }

        [Then(@"the response body Message should be empty")]
        public void ThenTheResponseBodyMessageShouldBeEmpty()
        {
            Assert.IsNotNull(_response, "Response should not be null");
            Assert.IsTrue(string.IsNullOrEmpty(_response.Message), 
                $"Expected empty Message but got '{_response.Message}'");
        }

        [Then(@"the response Data array should exist")]
        public void ThenTheResponseDataArrayShouldExist()
        {
            Assert.IsNotNull(_response, "Response should not be null");
            Assert.IsNotNull(_response.Data, "Data array should not be null");
        }

        [Then(@"the Data array length should be greater than (\d+)")]
        public void ThenTheDataArrayLengthShouldBeGreaterThan(int minLength)
        {
            Assert.IsNotNull(_response, "Response should not be null");
            Assert.IsNotNull(_response.Data, "Data array should not be null");
            Assert.Greater(_response.Data.Count, minLength, 
                $"Expected Data array length greater than {minLength} but got {_response.Data.Count}");
        }

        [Then(@"each item in the Data array should contain a cityid")]
        public void ThenEachItemInTheDataArrayShouldContainACityid()
        {
            Assert.IsNotNull(_response?.Data, "Data array should not be null");
            
            for (int i = 0; i < _response.Data.Count; i++)
            {
                var city = _response.Data[i];
                Assert.IsNotNull(city.CityId, $"City at index {i} should have a cityid");
                Assert.IsFalse(string.IsNullOrWhiteSpace(city.CityId), 
                    $"City at index {i} should have a non-empty cityid");
            }
        }

        [Then(@"each cityid should be a valid GUID")]
        public void ThenEachCityidShouldBeAValidGUID()
        {
            Assert.IsNotNull(_response?.Data, "Data array should not be null");
            
            for (int i = 0; i < _response.Data.Count; i++)
            {
                var city = _response.Data[i];
                bool isValidGuid = Guid.TryParse(city.CityId, out _);
                Assert.IsTrue(isValidGuid, 
                    $"City at index {i} has invalid GUID format: '{city.CityId}'");
            }
        }

        [Then(@"each item should contain a cityname")]
        public void ThenEachItemShouldContainACityname()
        {
            Assert.IsNotNull(_response?.Data, "Data array should not be null");
            
            for (int i = 0; i < _response.Data.Count; i++)
            {
                var city = _response.Data[i];
                Assert.IsNotNull(city.CityName, 
                    $"City at index {i} should have a cityname property");
            }
        }

        [Then(@"each cityname should be a non-empty string")]
        public void ThenEachCitynameShouldBeANonEmptyString()
        {
            Assert.IsNotNull(_response?.Data, "Data array should not be null");
            
            for (int i = 0; i < _response.Data.Count; i++)
            {
                var city = _response.Data[i];
                Assert.IsFalse(string.IsNullOrWhiteSpace(city.CityName), 
                    $"City at index {i} should have a non-empty cityname, but got '{city.CityName}'");
            }
        }

        [Then(@"there should be no duplicate cityid values")]
        public void ThenThereShouldBeNoDuplicateCityidValues()
        {
            Assert.IsNotNull(_response?.Data, "Data array should not be null");
            
            var cityIds = _response.Data.Select(c => c.CityId).ToList();
            var duplicates = cityIds.GroupBy(id => id)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();
            
            Assert.IsEmpty(duplicates, 
                $"Found duplicate cityid values: {string.Join(", ", duplicates)}");
        }

        [Then(@"the API response time should be less than (\d+) milliseconds")]
        public void ThenTheAPIResponseTimeShouldBeLessThanMilliseconds(int maxMilliseconds)
        {
            Assert.Less(_responseTimeMs, maxMilliseconds, 
                $"Expected response time less than {maxMilliseconds}ms but got {_responseTimeMs}ms");
        }
    }
}
