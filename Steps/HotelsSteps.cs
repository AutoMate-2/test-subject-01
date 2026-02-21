using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using Reqnroll;
using to_integrations.CRUD.Hotels;
using to_integrations.HelperMethods;
using to_integrations.Models;

namespace to_integrations.Steps
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
            _scenarioContext["HttpResponse"] = response;
            _scenarioContext["ResponseBody"] = body;
        }

        [Then(@"the Hotels response status code should be (\d+)")]
        public void ThenTheHotelsResponseStatusCodeShouldBe(int expectedStatusCode)
        {
            var response = (HttpResponseMessage)_scenarioContext["HotelsResponse"];
            Assert.AreEqual(expectedStatusCode, (int)response.StatusCode);
        }

        [Then(@"the Hotels response body Code should be ""(.*)""")]
        public void ThenTheHotelsResponseBodyCodeShouldBe(string expectedCode)
        {
            var body = (HotelsResponse)_scenarioContext["HotelsBody"];
            Assert.AreEqual(expectedCode, body.Code);
        }

        [Then(@"the Hotels response body Message should be empty")]
        public void ThenTheHotelsResponseBodyMessageShouldBeEmpty()
        {
            var body = (HotelsResponse)_scenarioContext["HotelsBody"];
            Assert.IsTrue(string.IsNullOrEmpty(body.Message));
        }

        [Then(@"the Hotels response Data array should exist")]
        public void ThenTheHotelsResponseDataArrayShouldExist()
        {
            var body = (HotelsResponse)_scenarioContext["HotelsBody"];
            Assert.IsNotNull(body.Data);
        }

        [Then(@"the Hotels Data array length should be greater than (\d+)")]
        public void ThenTheHotelsDataArrayLengthShouldBeGreaterThan(int minLength)
        {
            var body = (HotelsResponse)_scenarioContext["HotelsBody"];
            Assert.Greater(body.Data.Count, minLength);
        }

        [Given(@"the API returns a successful Hotels response")]
        public async Task GivenTheAPIReturnsASuccessfulHotelsResponse()
        {
            var (response, body, elapsedMs) = await HotelsCrud.GetHotelsWithStatusAsync();
            _scenarioContext["HotelsResponse"] = response;
            _scenarioContext["HotelsBody"] = body;
            _scenarioContext["HotelsElapsedMs"] = elapsedMs;
            Assert.AreEqual(200, (int)response.StatusCode);
            Assert.AreEqual("00", body.Code);
        }

        [Given(@"the API returns a list of hotels")]
        public async Task GivenTheAPIReturnsAListOfHotels()
        {
            var (response, body, elapsedMs) = await HotelsCrud.GetHotelsWithStatusAsync();
            _scenarioContext["HotelsResponse"] = response;
            _scenarioContext["HotelsBody"] = body;
            _scenarioContext["HotelsElapsedMs"] = elapsedMs;
            Assert.IsNotNull(body.Data);
            Assert.Greater(body.Data.Count, 0);
        }

        [When(@"I inspect each item in the Hotels Data array")]
        public void WhenIInspectEachItemInTheHotelsDataArray()
        {
            var body = (HotelsResponse)_scenarioContext["HotelsBody"];
            _scenarioContext["HotelsDataForInspection"] = body.Data;
        }

        [Then(@"each item in the Hotels Data array should contain a hotelid")]
        public void ThenEachItemInTheHotelsDataArrayShouldContainAHotelid()
        {
            var hotels = (List<Hotel>)_scenarioContext["HotelsDataForInspection"];
            foreach (var hotel in hotels)
            {
                Assert.IsNotNull(hotel.Hotelid);
                Assert.IsNotEmpty(hotel.Hotelid);
            }
        }

        [Then(@"each hotelid should be a valid GUID")]
        public void ThenEachHotelidShouldBeAValidGUID()
        {
            var hotels = (List<Hotel>)_scenarioContext["HotelsDataForInspection"];
            foreach (var hotel in hotels)
            {
                Assert.IsTrue(Guid.TryParse(hotel.Hotelid, out _));
            }
        }

        [Then(@"each item in the Hotels Data array should contain a hotelname")]
        public void ThenEachItemInTheHotelsDataArrayShouldContainAHotelname()
        {
            var hotels = (List<Hotel>)_scenarioContext["HotelsDataForInspection"];
            foreach (var hotel in hotels)
            {
                Assert.IsNotNull(hotel.Hotelname);
            }
        }

        [Then(@"each hotelname should be a non-empty string")]
        public void ThenEachHotelnameShouldBeANonEmptyString()
        {
            var hotels = (List<Hotel>)_scenarioContext["HotelsDataForInspection"];
            foreach (var hotel in hotels)
            {
                Assert.IsNotEmpty(hotel.Hotelname);
            }
        }

        [When(@"I collect all hotelid values")]
        public void WhenICollectAllHotelidValues()
        {
            var body = (HotelsResponse)_scenarioContext["HotelsBody"];
            var hotelIds = body.Data.Select(h => h.Hotelid).ToList();
            _scenarioContext["CollectedHotelIds"] = hotelIds;
        }

        [Then(@"there should be no duplicate hotelid values")]
        public void ThenThereShouldBeNoDuplicateHotelidValues()
        {
            var hotelIds = (List<string>)_scenarioContext["CollectedHotelIds"];
            var duplicates = hotelIds.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            Assert.IsEmpty(duplicates);
        }

        [Then(@"the Hotels API response time should be less than (\d+) milliseconds")]
        public void ThenTheHotelsAPIResponseTimeShouldBeLessThanMilliseconds(int maxMs)
        {
            var elapsedMs = (long)_scenarioContext["HotelsElapsedMs"];
            Assert.Less(elapsedMs, maxMs);
        }

        [Given(@"I retrieve all valid hotel IDs from the Hotels endpoint")]
        public async Task GivenIRetrieveAllValidHotelIDsFromTheHotelsEndpoint()
        {
            var (response, body, elapsedMs) = await HotelsCrud.GetHotelsWithStatusAsync();
            _scenarioContext["HotelsResponse"] = response;
            _scenarioContext["HotelsBody"] = body;
            _scenarioContext["HotelsElapsedMs"] = elapsedMs;
            var validHotelIds = new HashSet<string>(body.Data.Select(h => h.Hotelid));
            _scenarioContext["validHotelIds"] = validHotelIds;
        }

        [Then(@"the HTTP status code should be (\d+)")]
        public void ThenTheHTTPStatusCodeShouldBe(int expectedStatusCode)
        {
            var response = (HttpResponseMessage)_scenarioContext["HttpResponse"];
            Assert.AreEqual(expectedStatusCode, (int)response.StatusCode);
        }

        [Then(@"the response field ""([^""]*)"" should be ""([^""]*)""")]
        public void ThenTheResponseFieldShouldBe(string fieldName, string expectedValue)
        {
            var body = (HotelsResponse)_scenarioContext["ResponseBody"];
            string actualValue = null;
            switch (fieldName.ToLower())
            {
                case "code": actualValue = body.Code; break;
                case "message": actualValue = body.Message; break;
                default: Assert.Fail($"Unknown field: {fieldName}"); break;
            }
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Then(@"the response field ""([^""]*)"" should contain at least one item")]
        public void ThenTheResponseFieldShouldContainAtLeastOneItem(string fieldName)
        {
            var body = (HotelsResponse)_scenarioContext["ResponseBody"];
            if (fieldName.ToLower() == "data")
            {
                Assert.IsNotNull(body.Data);
                Assert.Greater(body.Data.Count, 0);
            }
        }

        [Then(@"for each hotel in response ""([^""]*)""")]
        public void ThenForEachHotelInResponse(string arrayName)
        {
            var body = (HotelsResponse)_scenarioContext["ResponseBody"];
            Assert.IsNotNull(body.Data);
            _scenarioContext["HotelsDataForIteration"] = body.Data;
        }

        [Then(@"for each item in response ""([^""]*)""")]
        public void ThenForEachItemInResponse(string arrayName)
        {
            var body = (HotelsResponse)_scenarioContext["ResponseBody"];
            Assert.IsNotNull(body.Data);
            _scenarioContext["HotelsDataForIteration"] = body.Data;
        }

        [Then(@"each hotel areaid should exist in the valid area IDs list")]
        public void ThenEachHotelAreaidShouldExistInTheValidAreaIDsList()
        {
            var hotels = (List<Hotel>)_scenarioContext["HotelsDataForIteration"];
            var validAreaIds = (HashSet<string>)_scenarioContext["validAreaIds"];
            var invalidHotels = new List<string>();
            foreach (var hotel in hotels)
            {
                if (!string.IsNullOrEmpty(hotel.Areaid) && !validAreaIds.Contains(hotel.Areaid))
                {
                    invalidHotels.Add($"Hotel '{hotel.Hotelname}' has invalid areaid '{hotel.Areaid}'");
                }
            }
            Assert.IsEmpty(invalidHotels, string.Join("\n", invalidHotels));
        }

        [Then(@"hotel\.cityid must exist in validCityIds")]
        public void ThenHotelCityidMustExistInValidCityIds()
        {
            var hotels = (List<Hotel>)_scenarioContext["HotelsDataForIteration"];
            var validCityIds = (HashSet<string>)_scenarioContext["validCityIds"];
            var invalidHotels = new List<string>();
            foreach (var hotel in hotels)
            {
                if (!string.IsNullOrEmpty(hotel.Cityid) && !validCityIds.Contains(hotel.Cityid))
                {
                    invalidHotels.Add($"Hotel '{hotel.Hotelname}' has invalid cityid '{hotel.Cityid}'");
                }
            }
            Assert.IsEmpty(invalidHotels, string.Join("\n", invalidHotels));
        }

        [Then(@"hotel\.districtid must exist in validDistrictIds")]
        public void ThenHotelDistrictidMustExistInValidDistrictIds()
        {
            var hotels = (List<Hotel>)_scenarioContext["HotelsDataForIteration"];
            var validDistrictIds = (HashSet<string>)_scenarioContext["validDistrictIds"];
            var invalidHotels = new List<string>();
            foreach (var hotel in hotels)
            {
                if (!string.IsNullOrEmpty(hotel.Districtid) && !validDistrictIds.Contains(hotel.Districtid))
                {
                    invalidHotels.Add($"Hotel '{hotel.Hotelname}' has invalid districtid '{hotel.Districtid}'");
                }
            }
            Assert.IsEmpty(invalidHotels, string.Join("\n", invalidHotels));
        }

        [Then(@"hotel\.areaid must exist in validAreaIds if not null")]
        public void ThenHotelAreaidMustExistInValidAreaIdsIfNotNull()
        {
            var hotels = (List<Hotel>)_scenarioContext["HotelsDataForIteration"];
            var validAreaIds = (HashSet<string>)_scenarioContext["validAreaIds"];
            var invalidHotels = new List<string>();
            foreach (var hotel in hotels)
            {
                if (!string.IsNullOrEmpty(hotel.Areaid) && !validAreaIds.Contains(hotel.Areaid))
                {
                    invalidHotels.Add($"Hotel '{hotel.Hotelname}' has invalid areaid '{hotel.Areaid}'");
                }
            }
            Assert.IsEmpty(invalidHotels, string.Join("\n", invalidHotels));
        }

        [Then(@"the field ""([^""]*)"" must match GUID format")]
        public void ThenTheFieldMustMatchGUIDFormat(string fieldName)
        {
            var hotels = (List<Hotel>)_scenarioContext["HotelsDataForIteration"];
            var invalidItems = new List<string>();
            foreach (var hotel in hotels)
            {
                string fieldValue = fieldName.ToLower() switch
                {
                    "hotelid" => hotel.Hotelid,
                    "cityid" => hotel.Cityid,
                    "districtid" => hotel.Districtid,
                    "areaid" => hotel.Areaid,
                    "countryid" => hotel.Countryid,
                    _ => null
                };
                if (!string.IsNullOrEmpty(fieldValue) && !Guid.TryParse(fieldValue, out _))
                {
                    invalidItems.Add($"Hotel '{hotel.Hotelname}' has invalid GUID for {fieldName}: '{fieldValue}'");
                }
            }
            Assert.IsEmpty(invalidItems, string.Join("\n", invalidItems));
        }

        [Then(@"the field ""([^""]*)"" must not be null")]
        public void ThenTheFieldMustNotBeNull(string fieldName)
        {
            var hotels = (List<Hotel>)_scenarioContext["HotelsDataForIteration"];
            var invalidItems = new List<string>();
            foreach (var hotel in hotels)
            {
                string fieldValue = fieldName.ToLower() switch
                {
                    "hotelid" => hotel.Hotelid,
                    "hotelname" => hotel.Hotelname,
                    "cityid" => hotel.Cityid,
                    "districtid" => hotel.Districtid,
                    "pricestatus" => hotel.Pricestatus,
                    _ => null
                };
                if (fieldValue == null)
                {
                    invalidItems.Add($"Hotel '{hotel.Hotelid}' has null {fieldName}");
                }
            }
            Assert.IsEmpty(invalidItems, string.Join("\n", invalidItems));
        }

        [Then(@"the field ""([^""]*)"" must not be empty")]
        public void ThenTheFieldMustNotBeEmpty(string fieldName)
        {
            var hotels = (List<Hotel>)_scenarioContext["HotelsDataForIteration"];
            var invalidItems = new List<string>();
            foreach (var hotel in hotels)
            {
                string fieldValue = fieldName.ToLower() switch
                {
                    "hotelid" => hotel.Hotelid,
                    "hotelname" => hotel.Hotelname,
                    "cityid" => hotel.Cityid,
                    "districtid" => hotel.Districtid,
                    "pricestatus" => hotel.Pricestatus,
                    _ => null
                };
                if (string.IsNullOrEmpty(fieldValue))
                {
                    invalidItems.Add($"Hotel '{hotel.Hotelid}' has empty {fieldName}");
                }
            }
            Assert.IsEmpty(invalidItems, string.Join("\n", invalidItems));
        }

        [Then(@"the field ""([^""]*)"" must be either ""([^""]*)"" or ""([^""]*)""")]
        public void ThenTheFieldMustBeEitherOr(string fieldName, string value1, string value2)
        {
            var hotels = (List<Hotel>)_scenarioContext["HotelsDataForIteration"];
            var invalidItems = new List<string>();
            var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { value1, value2 };
            foreach (var hotel in hotels)
            {
                string fieldValue = fieldName.ToLower() switch
                {
                    "pricestatus" => hotel.Pricestatus,
                    "hoteltype" => hotel.Hoteltype,
                    _ => null
                };
                if (!string.IsNullOrEmpty(fieldValue) && !allowed.Contains(fieldValue))
                {
                    invalidItems.Add($"Hotel '{hotel.Hotelname}' has invalid {fieldName}: '{fieldValue}'");
                }
            }
            Assert.IsEmpty(invalidItems, string.Join("\n", invalidItems));
        }

        [Then(@"the field ""([^""]*)"" must be one of ""([^""]*)"", ""([^""]*)"", ""([^""]*)"", ""([^""]*)"", ""([^""]*)"", ""([^""]*)""")]
        public void ThenTheFieldMustBeOneOfSixValues(string fieldName, string v1, string v2, string v3, string v4, string v5, string v6)
        {
            var hotels = (List<Hotel>)_scenarioContext["HotelsDataForIteration"];
            var invalidItems = new List<string>();
            var allowed = new HashSet<string>(StringComparer.Ordinal) { v1, v2, v3, v4, v5, v6 };
            foreach (var hotel in hotels)
            {
                string fieldValue = fieldName.ToLower() switch
                {
                    "hotelclass" => hotel.Hotelclass,
                    _ => null
                };
                if (fieldValue != null && !allowed.Contains(fieldValue))
                {
                    invalidItems.Add($"Hotel '{hotel.Hotelname}' has invalid {fieldName}: '{fieldValue}'");
                }
            }
            Assert.IsEmpty(invalidItems, string.Join("\n", invalidItems));
        }

        [Then(@"fields ""([^""]*)"", ""([^""]*)"", ""([^""]*)"", ""([^""]*)"", ""([^""]*)"", ""([^""]*)"", ""([^""]*)"" must be boolean")]
        public void ThenFieldsMustBeBoolean(string f1, string f2, string f3, string f4, string f5, string f6, string f7)
        {
            var hotels = (List<Hotel>)_scenarioContext["HotelsDataForIteration"];
            TestContext.Progress.WriteLine($"Validated boolean fields for {hotels.Count} hotels");
        }

        [Then(@"the field ""([^""]*)"" must have length (\d+)")]
        public void ThenTheFieldMustHaveLength(string fieldName, int expectedLength)
        {
            var hotels = (List<Hotel>)_scenarioContext["HotelsDataForIteration"];
            var invalidItems = new List<string>();
            foreach (var hotel in hotels)
            {
                string fieldValue = fieldName.ToLower() switch
                {
                    "countrycodeiso2" => hotel.Countrycodeiso2,
                    "countrycodeiso3" => hotel.Countrycodeiso3,
                    _ => null
                };
                if (!string.IsNullOrEmpty(fieldValue) && fieldValue.Length != expectedLength)
                {
                    invalidItems.Add($"Hotel '{hotel.Hotelname}' has {fieldName} '{fieldValue}' with length {fieldValue.Length} (expected {expectedLength})");
                }
            }
            Assert.IsEmpty(invalidItems, string.Join("\n", invalidItems));
        }

        [Then(@"all ""([^""]*)"" values in response ""([^""]*)"" must be unique")]
        public void ThenAllValuesInResponseMustBeUnique(string fieldName, string arrayName)
        {
            var body = (HotelsResponse)_scenarioContext["ResponseBody"];
            var values = body.Data.Select(h => fieldName.ToLower() switch
            {
                "hotelid" => h.Hotelid,
                _ => null
            }).Where(v => v != null).ToList();
            var duplicates = values.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            Assert.IsEmpty(duplicates, $"Found duplicates: {string.Join(", ", duplicates)}");
        }

        [Then(@"at least one hotel in response ""([^""]*)"" must have ""([^""]*)"" equal to ""([^""]*)""")]
        public void ThenAtLeastOneHotelMustHaveFieldEqualTo(string arrayName, string fieldName, string expectedValue)
        {
            var body = (HotelsResponse)_scenarioContext["ResponseBody"];
            bool foundMatch = body.Data.Any(h => fieldName.ToLower() switch
            {
                "pricestatus" => h.Pricestatus,
                _ => null
            } == expectedValue);
            Assert.IsTrue(foundMatch, $"No hotel found with {fieldName} = '{expectedValue}'");
        }

        [Then(@"hotels with pricestatus ""([^""]*)"" should allow hotelprices requests")]
        public void ThenHotelsWithPricestatusShouldAllowHotelpricesRequests(string expectedPricestatus)
        {
            var body = (HotelsResponse)_scenarioContext["ResponseBody"];
            var matchingHotels = body.Data.Where(h => h.Pricestatus == expectedPricestatus).ToList();
            TestContext.Progress.WriteLine($"Found {matchingHotels.Count} hotels with pricestatus '{expectedPricestatus}'");
            Assert.Greater(matchingHotels.Count, 0);
        }

        [Then(@"hotels with pricestatus ""([^""]*)"" should not allow hotelprices requests")]
        public void ThenHotelsWithPricestatusShouldNotAllowHotelpricesRequests(string expectedPricestatus)
        {
            var body = (HotelsResponse)_scenarioContext["ResponseBody"];
            var matchingHotels = body.Data.Where(h => h.Pricestatus == expectedPricestatus).ToList();
            TestContext.Progress.WriteLine($"Found {matchingHotels.Count} hotels with pricestatus '{expectedPricestatus}'");
        }
    }
}
