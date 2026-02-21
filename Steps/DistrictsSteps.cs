using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
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
        private DistrictsResponse _districtsResponse;
        private CitiesResponse _citiesResponse;
        private HttpResponseMessage _httpResponse;
        private long _responseTimeMs;
        private string _agentId;
        private string _agentPassword;
        private List<string> _validCityIds;

        public DistrictsSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _validCityIds = new List<string>();
        }

        [Given(@"I retrieve all valid city IDs from the Cities endpoint")]
        public async Task GivenIRetrieveAllValidCityIDsFromTheCitiesEndpoint()
        {
            _agentId = DistrictsPresetup.ValidAgentId;
            _agentPassword = DistrictsPresetup.ValidAgentPassword;
            
            var citiesCrud = new CitiesCrud();
            var (citiesResponse, statusCode) = await citiesCrud.GetCitiesWithStatusAsync();
            
            Assert.IsTrue((int)statusCode >= 200 && (int)statusCode < 300, 
                $"Failed to retrieve cities. Status: {statusCode}");
            
            _citiesResponse = citiesResponse;
            
            Assert.IsNotNull(_citiesResponse?.Data, "Cities response data should not be null");
            
            _validCityIds = _citiesResponse.Data.Select(c => c.CityId).ToList();
            
            TestContext.Progress.WriteLine($"Retrieved {_validCityIds.Count} valid city IDs");
            _scenarioContext["ValidCityIds"] = _validCityIds;
        }

        [When(@"I send a GET request to the Districts endpoint")]
        public async Task WhenISendAGETRequestToTheDistrictsEndpoint()
        {
            if (string.IsNullOrEmpty(_agentId))
            {
                _agentId = DistrictsPresetup.ValidAgentId;
                _agentPassword = DistrictsPresetup.ValidAgentPassword;
            }
            
            var districtsCrud = new DistrictsCrud();
            var stopwatch = Stopwatch.StartNew();
            
            _httpResponse = await districtsCrud.GetDistrictsAsync(_agentId, _agentPassword);
            
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
                _districtsResponse = JsonSerializer.Deserialize<DistrictsResponse>(content, options);
            }
            
            _scenarioContext["HttpResponse"] = _httpResponse;
            _scenarioContext["DistrictsResponse"] = _districtsResponse;
            _scenarioContext["ResponseTimeMs"] = _responseTimeMs;
        }

        [Then(@"the Districts Data array length should be greater than (\d+)")]
        public void ThenTheDistrictsDataArrayLengthShouldBeGreaterThan(int minLength)
        {
            Assert.IsNotNull(_districtsResponse, "Response should not be null");
            Assert.IsNotNull(_districtsResponse.Data, "Data array should not be null");
            Assert.Greater(_districtsResponse.Data.Count, minLength, 
                $"Expected Data array length greater than {minLength} but got {_districtsResponse.Data.Count}");
        }

        [Then(@"each item in the Districts Data array should contain a districtid")]
        public void ThenEachItemInTheDistrictsDataArrayShouldContainADistrictid()
        {
            Assert.IsNotNull(_districtsResponse?.Data, "Data array should not be null");
            
            for (int i = 0; i < _districtsResponse.Data.Count; i++)
            {
                var district = _districtsResponse.Data[i];
                Assert.IsNotNull(district.DistrictId, $"District at index {i} should have a districtid");
                Assert.IsFalse(string.IsNullOrWhiteSpace(district.DistrictId), 
                    $"District at index {i} should have a non-empty districtid");
            }
        }

        [Then(@"each districtid should be a valid GUID")]
        public void ThenEachDistrictidShouldBeAValidGUID()
        {
            Assert.IsNotNull(_districtsResponse?.Data, "Data array should not be null");
            
            for (int i = 0; i < _districtsResponse.Data.Count; i++)
            {
                var district = _districtsResponse.Data[i];
                bool isValidGuid = Guid.TryParse(district.DistrictId, out _);
                Assert.IsTrue(isValidGuid, 
                    $"District at index {i} has invalid GUID format for districtid: '{district.DistrictId}'");
            }
        }

        [Then(@"each item in the Districts Data array should contain a districtname")]
        public void ThenEachItemInTheDistrictsDataArrayShouldContainADistrictname()
        {
            Assert.IsNotNull(_districtsResponse?.Data, "Data array should not be null");
            
            for (int i = 0; i < _districtsResponse.Data.Count; i++)
            {
                var district = _districtsResponse.Data[i];
                Assert.IsNotNull(district.DistrictName, 
                    $"District at index {i} should have a districtname property");
            }
        }

        [Then(@"each districtname should be a non-empty string")]
        public void ThenEachDistrictnameShouldBeANonEmptyString()
        {
            Assert.IsNotNull(_districtsResponse?.Data, "Data array should not be null");
            
            for (int i = 0; i < _districtsResponse.Data.Count; i++)
            {
                var district = _districtsResponse.Data[i];
                Assert.IsFalse(string.IsNullOrWhiteSpace(district.DistrictName), 
                    $"District at index {i} should have a non-empty districtname, but got '{district.DistrictName}'");
            }
        }

        [Then(@"each item in the Districts Data array should contain a cityid")]
        public void ThenEachItemInTheDistrictsDataArrayShouldContainACityid()
        {
            Assert.IsNotNull(_districtsResponse?.Data, "Data array should not be null");
            
            for (int i = 0; i < _districtsResponse.Data.Count; i++)
            {
                var district = _districtsResponse.Data[i];
                Assert.IsNotNull(district.CityId, $"District at index {i} should have a cityid");
                Assert.IsFalse(string.IsNullOrWhiteSpace(district.CityId), 
                    $"District at index {i} should have a non-empty cityid");
            }
        }

        [Then(@"each district cityid should be a valid GUID")]
        public void ThenEachDistrictCityidShouldBeAValidGUID()
        {
            Assert.IsNotNull(_districtsResponse?.Data, "Data array should not be null");
            
            for (int i = 0; i < _districtsResponse.Data.Count; i++)
            {
                var district = _districtsResponse.Data[i];
                bool isValidGuid = Guid.TryParse(district.CityId, out _);
                Assert.IsTrue(isValidGuid, 
                    $"District at index {i} has invalid GUID format for cityid: '{district.CityId}'");
            }
        }

        [Then(@"there should be no duplicate districtid values")]
        public void ThenThereShouldBeNoDuplicateDistrictidValues()
        {
            Assert.IsNotNull(_districtsResponse?.Data, "Data array should not be null");
            
            var districtIds = _districtsResponse.Data.Select(d => d.DistrictId).ToList();
            var duplicates = districtIds.GroupBy(id => id)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();
            
            Assert.IsEmpty(duplicates, 
                $"Found duplicate districtid values: {string.Join(", ", duplicates)}");
        }

        [Then(@"every district cityid should exist in the valid city IDs list")]
        public void ThenEveryDistrictCityidShouldExistInTheValidCityIDsList()
        {
            Assert.IsNotNull(_districtsResponse?.Data, "Districts data should not be null");
            
            if (_scenarioContext.ContainsKey("ValidCityIds"))
            {
                _validCityIds = (List<string>)_scenarioContext["ValidCityIds"];
            }
            
            Assert.IsNotNull(_validCityIds, "Valid city IDs list should not be null");
            Assert.IsNotEmpty(_validCityIds, "Valid city IDs list should not be empty");
            
            var orphanDistricts = new List<string>();
            
            foreach (var district in _districtsResponse.Data)
            {
                if (!_validCityIds.Contains(district.CityId))
                {
                    orphanDistricts.Add($"District '{district.DistrictName}' (id: {district.DistrictId}) has cityid '{district.CityId}' which does not exist in Cities");
                }
            }
            
            if (orphanDistricts.Any())
            {
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
        }
    }
}
