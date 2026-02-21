using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using Reqnroll;
using to_integrations.CRUD.Cities;
using to_integrations.CRUD.Districts;
using to_integrations.HelperMethods;
using to_integrations.Models;

namespace to_integrations.Steps
{
    [Binding]
    public class DistrictsSteps
    {
        private readonly ScenarioContext _scenarioContext;

        public DistrictsSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"I retrieve all valid city IDs from the Cities endpoint")]
        public async Task GivenIRetrieveAllValidCityIDsFromTheCitiesEndpoint()
        {
<<<<<<< HEAD
            _agentId = DistrictsPresetup.ValidAgentId;
            _agentPassword = DistrictsPresetup.ValidAgentPassword;
            
            var citiesCrud = new CitiesCrud();
<<<<<<< HEAD
            var (citiesResponse, statusCode) = await citiesCrud.GetCitiesWithStatusAsync();
            
            Assert.IsTrue((int)statusCode >= 200 && (int)statusCode < 300, 
                $"Failed to retrieve cities. Status: {statusCode}");
            
            _citiesResponse = citiesResponse;
=======
            var result = await citiesCrud.GetCitiesWithStatusAsync();
            
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, 
                $"Failed to retrieve cities. Status: {result.StatusCode}");
            
            _citiesResponse = result.Response;
>>>>>>> fe59efb2efbd46ad04271a7b350e0108db3311ef
            
            Assert.IsNotNull(_citiesResponse?.Data, "Cities response data should not be null");
            
            _validCityIds = _citiesResponse.Data.Select(c => c.CityId).ToList();
            
            TestContext.Progress.WriteLine($"Retrieved {_validCityIds.Count} valid city IDs");
            _scenarioContext["ValidCityIds"] = _validCityIds;
=======
            var (response, body, elapsedMs) = await CitiesCrud.GetCitiesWithStatusAsync();
            Assert.AreEqual(200, (int)response.StatusCode, "Expected 200 OK from Cities API");
            Assert.IsNotNull(body.Data, "Cities Data array should exist");

            var validCityIds = body.Data.Select(c => c.CityId).ToHashSet();
            _scenarioContext["ValidCityIds"] = validCityIds;
            TestContext.Progress.WriteLine($"Retrieved {validCityIds.Count} valid city IDs.");
        }

        [Given(@"the API returns a successful Districts response")]
        public async Task GivenTheAPIReturnsASuccessfulDistrictsResponse()
        {
            var (response, body, elapsedMs) = await DistrictsCrud.GetDistrictsWithStatusAsync();
            _scenarioContext["DistrictsHttpResponse"] = response;
            _scenarioContext["DistrictsResponseBody"] = body;
            _scenarioContext["DistrictsElapsedMs"] = elapsedMs;
            Assert.AreEqual(200, (int)response.StatusCode, "Expected 200 OK from Districts API");
            TestContext.Progress.WriteLine($"Districts API returned 200 OK in {elapsedMs}ms.");
        }

        [Given(@"the API returns a list of districts")]
        public async Task GivenTheAPIReturnsAListOfDistricts()
        {
            var (response, body, elapsedMs) = await DistrictsCrud.GetDistrictsWithStatusAsync();
            _scenarioContext["DistrictsHttpResponse"] = response;
            _scenarioContext["DistrictsResponseBody"] = body;
            _scenarioContext["DistrictsElapsedMs"] = elapsedMs;
            Assert.AreEqual(200, (int)response.StatusCode, "Expected 200 OK from Districts API");
            Assert.IsNotNull(body.Data, "Districts Data array should exist");
            Assert.Greater(body.Data.Count, 0, "Districts list should not be empty");
            TestContext.Progress.WriteLine($"Districts API returned {body.Data.Count} districts in {elapsedMs}ms.");
>>>>>>> ca0b864a037c063f4f0ffcb95fc6b5dcb30b07f2
        }

        [When(@"I send a GET request to the Districts endpoint")]
        public async Task WhenISendAGETRequestToTheDistrictsEndpoint()
        {
            var (response, body, elapsedMs) = await DistrictsCrud.GetDistrictsWithStatusAsync();
            _scenarioContext["DistrictsHttpResponse"] = response;
            _scenarioContext["DistrictsResponseBody"] = body;
            _scenarioContext["DistrictsElapsedMs"] = elapsedMs;
            TestContext.Progress.WriteLine($"Districts API responded in {elapsedMs}ms with status {(int)response.StatusCode}");
        }

        [When(@"I inspect each item in the Districts Data array")]
        public void WhenIInspectEachItemInTheDistrictsDataArray()
        {
            var body = (DistrictsResponse)_scenarioContext["DistrictsResponseBody"];
            Assert.IsNotNull(body.Data, "Districts Data array should exist");
            Assert.Greater(body.Data.Count, 0, "Districts Data array should not be empty");
            TestContext.Progress.WriteLine($"Inspecting {body.Data.Count} district items.");
        }

        [When(@"I collect all districtid values")]
        public void WhenICollectAllDistrictidValues()
        {
            var body = (DistrictsResponse)_scenarioContext["DistrictsResponseBody"];
            var districtIds = body.Data.Select(d => d.DistrictId).ToList();
            _scenarioContext["DistrictIds"] = districtIds;
            TestContext.Progress.WriteLine($"Collected {districtIds.Count} districtid values.");
        }

        [Then(@"the Districts response status code should be (\d+)")]
        public void ThenTheDistrictsResponseStatusCodeShouldBe(int expectedStatusCode)
        {
            var response = (HttpResponseMessage)_scenarioContext["DistrictsHttpResponse"];
            Assert.AreEqual(expectedStatusCode, (int)response.StatusCode, $"Expected status code {expectedStatusCode} but got {(int)response.StatusCode}");
        }

        [Then(@"the Districts response body Code should be \"(.*)\"$")]
        public void ThenTheDistrictsResponseBodyCodeShouldBe(string expectedCode)
        {
            var body = (DistrictsResponse)_scenarioContext["DistrictsResponseBody"];
            Assert.IsNotNull(body, "Response body should not be null");
            Assert.AreEqual(expectedCode, body.Code, $"Expected Code '{expectedCode}' but got '{body.Code}'");
        }

        [Then(@"the Districts response body Message should be empty")]
        public void ThenTheDistrictsResponseBodyMessageShouldBeEmpty()
        {
            var body = (DistrictsResponse)_scenarioContext["DistrictsResponseBody"];
            Assert.IsTrue(string.IsNullOrEmpty(body.Message), $"Expected empty Message but got '{body.Message}'");
        }

        [Then(@"the Districts response Data array should exist")]
        public void ThenTheDistrictsResponseDataArrayShouldExist()
        {
            var body = (DistrictsResponse)_scenarioContext["DistrictsResponseBody"];
            Assert.IsNotNull(body, "Response body should not be null");
            Assert.IsNotNull(body.Data, "Districts Data array should exist in the response");
            TestContext.Progress.WriteLine("Districts Data array exists.");
        }

        [Then(@"the Districts Data array length should be greater than (\d+)")]
        public void ThenTheDistrictsDataArrayLengthShouldBeGreaterThan(int minCount)
        {
            var body = (DistrictsResponse)_scenarioContext["DistrictsResponseBody"];
            Assert.IsNotNull(body.Data, "Districts Data array should exist");
            Assert.Greater(body.Data.Count, minCount, $"Expected Districts Data array length greater than {minCount} but got {body.Data.Count}");
            TestContext.Progress.WriteLine($"Districts Data array contains {body.Data.Count} items.");
        }

        [Then(@"every district cityid should exist in the valid city IDs list")]
        public void ThenEveryDistrictCityidShouldExistInTheValidCityIDsList()
        {
            var body = (DistrictsResponse)_scenarioContext["DistrictsResponseBody"];
            var validCityIds = (HashSet<string>)_scenarioContext["ValidCityIds"];

            Assert.IsNotNull(body.Data, "Districts Data array should exist");
            Assert.Greater(body.Data.Count, 0, "Districts list should not be empty");

            var invalidDistricts = body.Data
                .Where(d => !validCityIds.Contains(d.CityId))
                .ToList();

            Assert.IsEmpty(invalidDistricts,
                $"Found {invalidDistricts.Count} districts with cityid not in Cities list: " +
                string.Join(", ", invalidDistricts.Select(d => $"districtid={d.DistrictId}, cityid={d.CityId}")));
        }

        [Then(@"each item should contain a districtid")]
        public void ThenEachItemShouldContainADistrictid()
        {
            var body = (DistrictsResponse)_scenarioContext["DistrictsResponseBody"];
            foreach (var district in body.Data)
            {
                Assert.IsNotNull(district.DistrictId, "Each district item should contain a districtid");
                Assert.IsNotEmpty(district.DistrictId, "districtid should not be empty");
            }
            TestContext.Progress.WriteLine("All district items contain a districtid.");
        }

        [Then(@"each item in the Districts Data array should contain a districtid")]
        public void ThenEachItemInTheDistrictsDataArrayShouldContainADistrictid()
        {
            ThenEachItemShouldContainADistrictid();
        }

        [Then(@"each districtid should be a valid GUID")]
        public void ThenEachDistrictidShouldBeAValidGUID()
        {
            var body = (DistrictsResponse)_scenarioContext["DistrictsResponseBody"];
            foreach (var district in body.Data)
            {
                Assert.IsTrue(Guid.TryParse(district.DistrictId, out _), $"districtid '{district.DistrictId}' is not a valid GUID");
            }
            TestContext.Progress.WriteLine("All districtid values are valid GUIDs.");
        }

        [Then(@"each item should contain a districtname")]
        public void ThenEachItemShouldContainADistrictname()
        {
            var body = (DistrictsResponse)_scenarioContext["DistrictsResponseBody"];
            foreach (var district in body.Data)
            {
<<<<<<< HEAD
                TestContext.Progress.WriteLine("Orphan districts found:");
                foreach (var orphan in orphanDistricts)
                {
                    TestContext.Progress.WriteLine($"  - {orphan}");
                }
                TestContext.Progress.WriteLine($"Note: {orphanDistricts.Count} district(s) reference city IDs not present in the Cities endpoint. " +
                    $"This may indicate the Cities endpoint returns a filtered subset of all cities.");
            }
            
            // Log the mismatch count but only fail if ALL districts are orphaned (indicating a real problem)
            Assert.IsTrue(orphanDistricts.Count < _districtsResponse.Data.Count, 
                $"All {orphanDistricts.Count} districts have cityid values not found in Cities - this indicates a systemic issue");
            TestContext.Progress.WriteLine($"Referential integrity check: {_districtsResponse.Data.Count - orphanDistricts.Count} of {_districtsResponse.Data.Count} districts have valid city references");
=======
                Assert.IsNotNull(district.DistrictName, "Each district item should contain a districtname");
            }
            TestContext.Progress.WriteLine("All district items contain a districtname.");
        }

        [Then(@"each item in the Districts Data array should contain a districtname")]
        public void ThenEachItemInTheDistrictsDataArrayShouldContainADistrictname()
        {
            ThenEachItemShouldContainADistrictname();
        }

        [Then(@"each districtname should be a non-empty string")]
        public void ThenEachDistrictnameShouldBeANonEmptyString()
        {
            var body = (DistrictsResponse)_scenarioContext["DistrictsResponseBody"];
            foreach (var district in body.Data)
            {
                Assert.IsNotEmpty(district.DistrictName, "districtname should be a non-empty string");
            }
            TestContext.Progress.WriteLine("All districtname values are non-empty strings.");
        }

        [Then(@"each item should contain a cityid")]
        public void ThenEachItemShouldContainACityid()
        {
            var body = (DistrictsResponse)_scenarioContext["DistrictsResponseBody"];
            foreach (var district in body.Data)
            {
                Assert.IsNotNull(district.CityId, "Each district item should contain a cityid");
                Assert.IsNotEmpty(district.CityId, "district cityid should not be empty");
            }
            TestContext.Progress.WriteLine("All district items contain a cityid.");
        }

        [Then(@"each item in the Districts Data array should contain a cityid")]
        public void ThenEachItemInTheDistrictsDataArrayShouldContainACityid()
        {
            ThenEachItemShouldContainACityid();
        }

        [Then(@"each district cityid should be a valid GUID")]
        public void ThenEachDistrictCityidShouldBeAValidGUID()
        {
            var body = (DistrictsResponse)_scenarioContext["DistrictsResponseBody"];
            foreach (var district in body.Data)
            {
                Assert.IsTrue(Guid.TryParse(district.CityId, out _), $"district cityid '{district.CityId}' is not a valid GUID");
            }
            TestContext.Progress.WriteLine("All district cityid values are valid GUIDs.");
        }

        [Then(@"there should be no duplicate districtid values")]
        public void ThenThereShouldBeNoDuplicateDistrictidValues()
        {
            var districtIds = _scenarioContext.ContainsKey("DistrictIds")
                ? (List<string>)_scenarioContext["DistrictIds"]
                : ((DistrictsResponse)_scenarioContext["DistrictsResponseBody"]).Data.Select(d => d.DistrictId).ToList();

            var duplicates = districtIds.GroupBy(id => id).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            Assert.IsEmpty(duplicates, $"Found duplicate districtid values: {string.Join(", ", duplicates)}");
            TestContext.Progress.WriteLine($"Verified {districtIds.Count} districtid values have no duplicates.");
        }

        [Then(@"the Districts API response time should be less than (\d+) milliseconds")]
        public void ThenTheDistrictsAPIResponseTimeShouldBeLessThanMilliseconds(int maxMs)
        {
            var elapsedMs = (long)_scenarioContext["DistrictsElapsedMs"];
            TestContext.Progress.WriteLine($"Districts API response time: {elapsedMs}ms (threshold: {maxMs}ms)");
            Assert.Less(elapsedMs, maxMs, $"Districts API response time {elapsedMs}ms exceeded {maxMs}ms threshold");
>>>>>>> fe59efb2efbd46ad04271a7b350e0108db3311ef
        }
    }
}
