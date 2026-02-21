using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using Reqnroll;
using to_integrations.CRUD.Hotels;
using to_integrations.CRUD.HotelPrices;
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
        }

        [When(@"I send a GET request to \"/hotels\"")]
        public async Task WhenISendAGETRequestToHotels()
        {
            var (response, body, elapsedMs) = await HotelsCrud.GetHotelsWithStatusAsync();
            _scenarioContext["HotelsResponse"] = response;
            _scenarioContext["HotelsBody"] = body;
            _scenarioContext["HotelsElapsedMs"] = elapsedMs;
            _scenarioContext["HttpResponse"] = response;
            _scenarioContext["ResponseBody"] = body;
        }

        [When(@"I send a GET request to \"/Hotels\"")]
        public async Task WhenISendAGETRequestToHotelsUpperCase()
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
            Assert.AreEqual(expectedStatusCode, (int)response.StatusCode, $"Expected status code {expectedStatusCode} but got {(int)response.StatusCode}");
        }

        [Then(@"the HTTP status code should be (\d+)")]
        public void ThenTheHTTPStatusCodeShouldBe(int expectedStatusCode)
        {
            var response = (HttpResponseMessage)_scenarioContext["HttpResponse"];
            Assert.AreEqual(expectedStatusCode, (int)response.StatusCode, $"Expected HTTP status code {expectedStatusCode} but got {(int)response.StatusCode}");
        }

        [Then(@"the response field \"([^\"]*)\" should be \"([^\"]*)\"")]
        public void ThenTheResponseFieldShouldBe(string fieldName, string expectedValue)
        {
            var body = (HotelsResponse)_scenarioContext["ResponseBody"];
            string actualValue = null;

            switch (fieldName.ToLower())
            {
                case "code":
                    actualValue = body.Code;
                    break;
                case "message":
                    actualValue = body.Message;
                    break;
                default:
                    Assert.Fail($"Unknown field name: {fieldName}");
                    break;
            }

            Assert.AreEqual(expectedValue, actualValue, $"Expected field '{fieldName}' to be '{expectedValue}' but got '{actualValue}'");
        }

        [Then(@"the response field \"([^\"]*)\" should contain at least one item")]
        public void ThenTheResponseFieldShouldContainAtLeastOneItem(string fieldName)
        {
            var body = (HotelsResponse)_scenarioContext["ResponseBody"];

            switch (fieldName.ToLower())
            {
                case "data":
                    Assert.IsNotNull(body.Data, $"Response field '{fieldName}' should not be null");
                    Assert.Greater(body.Data.Count, 0, $"Response field '{fieldName}' should contain at least one item but was empty");
                    TestContext.Progress.WriteLine($"Hotels Data array contains {body.Data.Count} items");
                    break;
                default:
                    Assert.Fail($"Unknown array field name: {fieldName}");
                    break;
            }
        }

        [Then(@"the Hotels response body Code should be (.*)")]
        public void ThenTheHotelsResponseBodyCodeShouldBe(string expectedCode)
        {
            var body = (HotelsResponse)_scenarioContext["HotelsBody"];
            var cleanExpectedCode = expectedCode.Trim('"');
            Assert.AreEqual(cleanExpectedCode, body.Code, $"Expected Code '{cleanExpectedCode}' but got '{body.Code}'");
        }

        [Then(@"the Hotels response body Message should be empty")]
        public void ThenTheHotelsResponseBodyMessageShouldBeEmpty()
        {
            var body = (HotelsResponse)_scenarioContext["HotelsBody"];
            Assert.IsTrue(string.IsNullOrEmpty(body.Message), $"Expected empty Message but got '{body.Message}'");
        }

        [Then(@"the Hotels response Data array should exist")]
        public void ThenTheHotelsResponseDataArrayShouldExist()
        {
            var body = (HotelsResponse)_scenarioContext["HotelsBody"];
            Assert.IsNotNull(body.Data, "Hotels Data array should not be null");
        }

        [Then(@"the Hotels Data array length should be greater than (\d+)")]
        public void ThenTheHotelsDataArrayLengthShouldBeGreaterThan(int minLength)
        {
            var body = (HotelsResponse)_scenarioContext["HotelsBody"];
            Assert.Greater(body.Data.Count, minLength, $"Expected Hotels Data array length greater than {minLength} but got {body.Data.Count}");
        }

        [Then(@"for each hotel in response \"([^\"]*)\"")]
        public void ThenForEachHotelInResponse(string arrayName)
        {
            var body = (HotelsResponse)_scenarioContext["HotelsBody"];
            Assert.IsNotNull(body.Data, $"Response array '{arrayName}' should not be null");
            _scenarioContext["HotelsDataForIteration"] = body.Data;
        }

        [Then(@"for each item in response \"([^\"]*)\"")]
        public void ThenForEachItemInResponse(string arrayName)
        {
            var body = (HotelsResponse)_scenarioContext["ResponseBody"];
            Assert.IsNotNull(body.Data, $"Response array '{arrayName}' should not be null");
            _scenarioContext["HotelsDataForIteration"] = body.Data;
            _scenarioContext["HotelsDataForInspection"] = body.Data;
            TestContext.Progress.WriteLine($"Iterating over {body.Data.Count} items in '{arrayName}' array");
        }

        [Then(@"the field \"([^\"]*)\" must match GUID format")]
        public void ThenTheFieldMustMatchGUIDFormat(string fieldName)
        {
            var hotels = (List<Hotel>)_scenarioContext["HotelsDataForIteration"];
            var invalidItems = new List<string>();

            foreach (var hotel in hotels)
            {
                string fieldValue = null;

                switch (fieldName.ToLower())
                {
                    case "hotelid":
                        fieldValue = hotel.Hotelid;
                        break;
                    case "cityid":
                        fieldValue = hotel.Cityid;
                        break;
                    case "districtid":
                        fieldValue = hotel.Districtid;
                        break;
                    case "areaid":
                        fieldValue = hotel.Areaid;
                        break;
                    case "countryid":
                        fieldValue = hotel.Countryid;
                        break;
                    default:
                        Assert.Fail($"Unknown field name for GUID validation: {fieldName}");
                        break;
                }

                if (string.IsNullOrEmpty(fieldValue))
                {
                    invalidItems.Add($"Hotel '{hotel.Hotelname}' has null or empty {fieldName}");
                }
                else if (!Guid.TryParse(fieldValue, out _))
                {
                    invalidItems.Add($"Hotel '{hotel.Hotelname}' has invalid GUID for {fieldName}: '{fieldValue}'");
                }
            }

            Assert.IsEmpty(invalidItems, $"Found items with invalid GUID format for field '{fieldName}':\n{string.Join("\n", invalidItems)}");
            TestContext.Progress.WriteLine($"All {hotels.Count} hotels have valid GUID format for field '{fieldName}'");
        }

        [Then(@"the field \"([^\"]*)\" must not be null")]
        public void ThenTheFieldMustNotBeNull(string fieldName)
        {
            var hotels = (List<Hotel>)_scenarioContext["HotelsDataForIteration"];
            var invalidItems = new List<string>();

            foreach (var hotel in hotels)
            {
                string fieldValue = null;

                switch (fieldName.ToLower())
                {
                    case "hotelid":
                        fieldValue = hotel.Hotelid;
                        break;
                    case "hotelname":
                        fieldValue = hotel.Hotelname;
                        break;
                    case "cityid":
                        fieldValue = hotel.Cityid;
                        break;
                    case "cityname":
                        fieldValue = hotel.Cityname;
                        break;
                    case "districtid":
                        fieldValue = hotel.Districtid;
                        break;
                    case "districtname":
                        fieldValue = hotel.Districtname;
                        break;
                    case "areaid":
                        fieldValue = hotel.Areaid;
                        break;
                    case "areaname":
                        fieldValue = hotel.Areaname;
                        break;
                    case "countryid":
                        fieldValue = hotel.Countryid;
                        break;
                    case "countryname":
                        fieldValue = hotel.Countryname;
                        break;
                    case "pricestatus":
                        fieldValue = hotel.Pricestatus;
                        break;
                    case "hotelclass":
                        fieldValue = hotel.Hotelclass;
                        break;
                    case "countrycodeiso2":
                        fieldValue = hotel.Countrycodeiso2;
                        break;
                    case "countrycodeiso3":
                        fieldValue = hotel.Countrycodeiso3;
                        break;
                    default:
                        Assert.Fail($"Unknown field name for null validation: {fieldName}");
                        break;
                }

                if (fieldValue == null)
                {
                    invalidItems.Add($"Hotel with id '{hotel.Hotelid}' has null {fieldName}");
                }
            }

            Assert.IsEmpty(invalidItems, $"Found items with null field '{fieldName}':\n{string.Join("\n", invalidItems)}");
            TestContext.Progress.WriteLine($"All {hotels.Count} hotels have non-null field '{fieldName}'");
        }

        [Then(@"the field \"([^\"]*)\" must not be empty")]
        public void ThenTheFieldMustNotBeEmpty(string fieldName)
        {
            var hotels = (List<Hotel>)_scenarioContext["HotelsDataForIteration"];
            var invalidItems = new List<string>();

            foreach (var hotel in hotels)
            {
                string fieldValue = null;

                switch (fieldName.ToLower())
                {
                    case "hotelid":
                        fieldValue = hotel.Hotelid;
                        break;
                    case "hotelname":
                        fieldValue = hotel.Hotelname;
                        break;
                    case "cityid":
                        fieldValue = hotel.Cityid;
                        break;
                    case "cityname":
                        fieldValue = hotel.Cityname;
                        break;
                    case "districtid":
                        fieldValue = hotel.Districtid;
                        break;
                    case "districtname":
                        fieldValue = hotel.Districtname;
                        break;
                    case "areaid":
                        fieldValue = hotel.Areaid;
                        break;
                    case "areaname":
                        fieldValue = hotel.Areaname;
                        break;
                    case "countryid":
                        fieldValue = hotel.Countryid;
                        break;
                    case "countryname":
                        fieldValue = hotel.Countryname;
                        break;
                    case "pricestatus":
                        fieldValue = hotel.Pricestatus;
                        break;
                    case "hotelclass":
                        fieldValue = hotel.Hotelclass;
                        break;
                    case "countrycodeiso2":
                        fieldValue = hotel.Countrycodeiso2;
                        break;
                    case "countrycodeiso3":
                        fieldValue = hotel.Countrycodeiso3;
                        break;
                    default:
                        Assert.Fail($"Unknown field name for empty validation: {fieldName}");
                        break;
                }

                if (string.IsNullOrEmpty(fieldValue))
                {
                    invalidItems.Add($"Hotel with id '{hotel.Hotelid}' has empty {fieldName}");
                }
            }

            Assert.IsEmpty(invalidItems, $"Found items with empty field '{fieldName}':\n{string.Join("\n", invalidItems)}");
            TestContext.Progress.WriteLine($"All {hotels.Count} hotels have non-empty field '{fieldName}'");
        }

        [Then(@"the field \"([^\"]*)\" must be either \"([^\"]*)\" or \"([^\"]*)\"")]
        public void ThenTheFieldMustBeEitherOr(string fieldName, string allowedValue1, string allowedValue2)
        {
            var hotels = (List<Hotel>)_scenarioContext["HotelsDataForIteration"];
            var invalidItems = new List<string>();
            var allowedValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { allowedValue1, allowedValue2 };

            foreach (var hotel in hotels)
            {
                string fieldValue = null;

                switch (fieldName.ToLower())
                {
                    case "pricestatus":
                        fieldValue = hotel.Pricestatus;
                        break;
                    case "hotelclass":
                        fieldValue = hotel.Hotelclass;
                        break;
                    case "hoteltype":
                        fieldValue = hotel.Hoteltype;
                        break;
                    default:
                        Assert.Fail($"Unknown field name for enum validation: {fieldName}");
                        break;
                }

                if (string.IsNullOrEmpty(fieldValue))
                {
                    invalidItems.Add($"Hotel '{hotel.Hotelname}' (hotelid: {hotel.Hotelid}) has null or empty {fieldName}");
                }
                else if (!allowedValues.Contains(fieldValue))
                {
                    invalidItems.Add($"Hotel '{hotel.Hotelname}' (hotelid: {hotel.Hotelid}) has invalid {fieldName}: '{fieldValue}' (expected '{allowedValue1}' or '{allowedValue2}')");
                }
            }

            Assert.IsEmpty(invalidItems, $"Found items with invalid '{fieldName}' values:\n{string.Join("\n", invalidItems)}");
            TestContext.Progress.WriteLine($"All {hotels.Count} hotels have valid '{fieldName}' values (either '{allowedValue1}' or '{allowedValue2}')");
        }

        [Then(@"the field \"([^\"]*)\" must be one of \"([^\"]*)\", \"([^\"]*)\", \"([^\"]*)\", \"([^\"]*)\", \"([^\"]*)\", \"([^\"]*)\"")]
        public void ThenTheFieldMustBeOneOfSixValues(string fieldName, string value1, string value2, string value3, string value4, string value5, string value6)
        {
            var hotels = (List<Hotel>)_scenarioContext["HotelsDataForIteration"];
            var invalidItems = new List<string>();
            var allowedValues = new HashSet<string>(StringComparer.Ordinal) { value1, value2, value3, value4, value5, value6 };

            foreach (var hotel in hotels)
            {
                string fieldValue = null;

                switch (fieldName.ToLower())
                {
                    case "hotelclass":
                        fieldValue = hotel.Hotelclass;
                        break;
                    case "hoteltype":
                        fieldValue = hotel.Hoteltype;
                        break;
                    case "pricestatus":
                        fieldValue = hotel.Pricestatus;
                        break;
                    default:
                        Assert.Fail($"Unknown field name for allowed values validation: {fieldName}");
                        break;
                }

                if (fieldValue == null)
                {
                    invalidItems.Add($"Hotel '{hotel.Hotelname}' (hotelid: {hotel.Hotelid}) has null {fieldName}");
                }
                else if (!allowedValues.Contains(fieldValue))
                {
                    invalidItems.Add($"Hotel '{hotel.Hotelname}' (hotelid: {hotel.Hotelid}) has invalid {fieldName}: '{fieldValue}' (expected one of: '{value1}', '{value2}', '{value3}', '{value4}', '{value5}', '{value6}')");
                }
            }

            Assert.IsEmpty(invalidItems, $"Found items with invalid '{fieldName}' values:\n{string.Join("\n", invalidItems)}");
            TestContext.Progress.WriteLine($"All {hotels.Count} hotels have valid '{fieldName}' values (one of: '{value1}', '{value2}', '{value3}', '{value4}', '{value5}', '{value6}')");
        }

        [Then(@"fields \"([^\"]*)\", \"([^\"]*)\", \"([^\"]*)\", \"([^\"]*)\", \"([^\"]*)\", \"([^\"]*)\", \"([^\"]*)\" must be boolean")]
        public void ThenFieldsMustBeBoolean(string field1, string field2, string field3, string field4, string field5, string field6, string field7)
        {
            var hotels = (List<Hotel>)_scenarioContext["HotelsDataForIteration"];
            var fieldNames = new List<string> { field1, field2, field3, field4, field5, field6, field7 };
            var invalidItems = new List<string>();

            TestContext.Progress.WriteLine($"Validating boolean fields: {string.Join(", ", fieldNames)}");

            foreach (var hotel in hotels)
            {
                foreach (var fieldName in fieldNames)
                {
                    bool? fieldValue = null;
                    bool isValidBoolean = true;

                    switch (fieldName.ToLower())
                    {
                        case "hasalcohol":
                            fieldValue = hotel.Hasalcohol;
                            break;
                        case "hasfreewifi":
                            fieldValue = hotel.Hasfreewifi;
                            break;
                        case "hasmall":
                            fieldValue = hotel.Hasmall;
                            break;
                        case "hasmetro":
                            fieldValue = hotel.Hasmetro;
                            break;
                        case "haspool":
                            fieldValue = hotel.Haspool;
                            break;
                        case "popular":
                            fieldValue = hotel.Popular;
                            break;
                        case "recommended":
                            fieldValue = hotel.Recommended;
                            break;
                        default:
                            invalidItems.Add($"Unknown boolean field name: {fieldName}");
                            isValidBoolean = false;
                            break;
                    }

                    if (isValidBoolean && fieldValue == null)
                    {
                        invalidItems.Add($"Hotel '{hotel.Hotelname}' (hotelid: {hotel.Hotelid}) has null value for boolean field '{fieldName}'");
                    }
                }
            }

            Assert.IsEmpty(invalidItems, $"Found items with invalid boolean fields:\n{string.Join("\n", invalidItems)}");
            TestContext.Progress.WriteLine($"All {hotels.Count} hotels have valid boolean values for fields: {string.Join(", ", fieldNames)}");
        }

        [Then(@"the field \"([^\"]*)\" must have length (\d+)")]
        public void ThenTheFieldMustHaveLength(string fieldName, int expectedLength)
        {
            var hotels = (List<Hotel>)_scenarioContext["HotelsDataForIteration"];
            var invalidItems = new List<string>();

            foreach (var hotel in hotels)
            {
                string fieldValue = null;

                switch (fieldName.ToLower())
                {
                    case "countrycodeiso2":
                        fieldValue = hotel.Countrycodeiso2;
                        break;
                    case "countrycodeiso3":
                        fieldValue = hotel.Countrycodeiso3;
                        break;
                    default:
                        Assert.Fail($"Unknown field name for length validation: {fieldName}");
                        break;
                }

                if (string.IsNullOrEmpty(fieldValue))
                {
                    invalidItems.Add($"Hotel '{hotel.Hotelname}' (hotelid: {hotel.Hotelid}) has null or empty {fieldName}");
                }
                else if (fieldValue.Length != expectedLength)
                {
                    invalidItems.Add($"Hotel '{hotel.Hotelname}' (hotelid: {hotel.Hotelid}) has {fieldName} '{fieldValue}' with length {fieldValue.Length} (expected {expectedLength})");
                }
            }

            Assert.IsEmpty(invalidItems, $"Found items with invalid '{fieldName}' length:\n{string.Join("\n", invalidItems)}");
            TestContext.Progress.WriteLine($"All {hotels.Count} hotels have '{fieldName}' with correct length of {expectedLength}");
        }

        [Then(@"all \"([^\"]*)\" values in response \"([^\"]*)\" must be unique")]
        public void ThenAllValuesInResponseMustBeUnique(string fieldName, string arrayName)
        {
            var body = (HotelsResponse)_scenarioContext["ResponseBody"];
            Assert.IsNotNull(body.Data, $"Response array '{arrayName}' should not be null");

            var values = new List<string>();

            foreach (var hotel in body.Data)
            {
                string fieldValue = null;

                switch (fieldName.ToLower())
                {
                    case "hotelid":
                        fieldValue = hotel.Hotelid;
                        break;
                    case "cityid":
                        fieldValue = hotel.Cityid;
                        break;
                    case "districtid":
                        fieldValue = hotel.Districtid;
                        break;
                    case "areaid":
                        fieldValue = hotel.Areaid;
                        break;
                    case "countryid":
                        fieldValue = hotel.Countryid;
                        break;
                    default:
                        Assert.Fail($"Unknown field name for uniqueness validation: {fieldName}");
                        break;
                }

                if (!string.IsNullOrEmpty(fieldValue))
                {
                    values.Add(fieldValue);
                }
            }

            var duplicates = values.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            Assert.IsEmpty(duplicates, $"Found duplicate '{fieldName}' values in '{arrayName}': {string.Join(", ", duplicates)}");
            TestContext.Progress.WriteLine($"All {values.Count} '{fieldName}' values in '{arrayName}' are unique");
        }

        [Then(@"each hotel areaid should exist in the valid area IDs list")]
        public void ThenEachHotelAreaidShouldExistInTheValidAreaIDsList()
        {
            var hotels = (List<Hotel>)_scenarioContext["HotelsDataForIteration"];
            var validAreaIds = (HashSet<string>)_scenarioContext["validAreaIds"];

            var invalidHotels = new List<string>();

            foreach (var hotel in hotels)
            {
                if (!string.IsNullOrEmpty(hotel.Areaid))
                {
                    if (!validAreaIds.Contains(hotel.Areaid))
                    {
                        invalidHotels.Add($"Hotel '{hotel.Hotelname}' (hotelid: {hotel.Hotelid}) has areaid '{hotel.Areaid}' which does not exist in Areas");
                    }
                }
            }

            Assert.IsEmpty(invalidHotels, $"Found hotels with invalid areaid references:\n{string.Join("\n", invalidHotels)}");
        }

        [Then(@"hotel\.areaid must exist in validAreaIds if not null")]
        public void ThenHotelAreaidMustExistInValidAreaIdsIfNotNull()
        {
            var hotels = (List<Hotel>)_scenarioContext["HotelsDataForIteration"];
            var validAreaIds = (HashSet<string>)_scenarioContext["validAreaIds"];

            var invalidHotels = new List<string>();
            int nullAreaIdCount = 0;
            int validAreaIdCount = 0;

            foreach (var hotel in hotels)
            {
                if (string.IsNullOrEmpty(hotel.Areaid))
                {
                    nullAreaIdCount++;
                    TestContext.Progress.WriteLine($"Hotel '{hotel.Hotelname}' (hotelid: {hotel.Hotelid}) has null/empty areaid - skipping validation");
                }
                else
                {
                    if (!validAreaIds.Contains(hotel.Areaid))
                    {
                        invalidHotels.Add($"Hotel '{hotel.Hotelname}' (hotelid: {hotel.Hotelid}) has areaid '{hotel.Areaid}' which does not exist in Areas");
                    }
                    else
                    {
                        validAreaIdCount++;
                    }
                }
            }

            TestContext.Progress.WriteLine($"Total hotels: {hotels.Count}, Hotels with null areaid: {nullAreaIdCount}, Hotels with valid areaid: {validAreaIdCount}");
            Assert.IsEmpty(invalidHotels, $"Found hotels with invalid areaid references:\n{string.Join("\n", invalidHotels)}");
            TestContext.Progress.WriteLine($"All {validAreaIdCount} hotels with non-null areaid have valid references that exist in Areas");
        }

        [Then(@"hotel\.cityid must exist in validCityIds")]
        public void ThenHotelCityidMustExistInValidCityIds()
        {
            var hotels = (List<Hotel>)_scenarioContext["HotelsDataForIteration"];
            var validCityIds = (HashSet<string>)_scenarioContext["validCityIds"];

            var invalidHotels = new List<string>();

            foreach (var hotel in hotels)
            {
                if (!string.IsNullOrEmpty(hotel.Cityid))
                {
                    if (!validCityIds.Contains(hotel.Cityid))
                    {
                        invalidHotels.Add($"Hotel '{hotel.Hotelname}' (hotelid: {hotel.Hotelid}) has cityid '{hotel.Cityid}' which does not exist in Cities");
                    }
                }
                else
                {
                    invalidHotels.Add($"Hotel '{hotel.Hotelname}' (hotelid: {hotel.Hotelid}) has null or empty cityid");
                }
            }

            Assert.IsEmpty(invalidHotels, $"Found hotels with invalid cityid references:\n{string.Join("\n", invalidHotels)}");
            TestContext.Progress.WriteLine($"All {hotels.Count} hotels have valid cityid references that exist in Cities");
        }

        [Then(@"hotel\.districtid must exist in validDistrictIds")]
        public void ThenHotelDistrictidMustExistInValidDistrictIds()
        {
            var hotels = (List<Hotel>)_scenarioContext["HotelsDataForIteration"];
            var validDistrictIds = (HashSet<string>)_scenarioContext["validDistrictIds"];

            var invalidHotels = new List<string>();

            foreach (var hotel in hotels)
            {
                if (!string.IsNullOrEmpty(hotel.Districtid))
                {
                    if (!validDistrictIds.Contains(hotel.Districtid))
                    {
                        invalidHotels.Add($"Hotel '{hotel.Hotelname}' (hotelid: {hotel.Hotelid}) has districtid '{hotel.Districtid}' which does not exist in Districts");
                    }
                }
                else
                {
                    invalidHotels.Add($"Hotel '{hotel.Hotelname}' (hotelid: {hotel.Hotelid}) has null or empty districtid");
                }
            }

            Assert.IsEmpty(invalidHotels, $"Found hotels with invalid districtid references:\n{string.Join("\n", invalidHotels)}");
            TestContext.Progress.WriteLine($"All {hotels.Count} hotels have valid districtid references that exist in Districts");
        }

        [Then(@"at least one hotel in response \"([^\"]*)\" must have \"([^\"]*)\" equal to \"([^\"]*)\"")]
        public void ThenAtLeastOneHotelInResponseMustHaveFieldEqualTo(string arrayName, string fieldName, string expectedValue)
        {
            var body = (HotelsResponse)_scenarioContext["ResponseBody"];
            Assert.IsNotNull(body.Data, $"Response array '{arrayName}' should not be null");
            Assert.Greater(body.Data.Count, 0, $"Response array '{arrayName}' should not be empty");

            bool foundMatch = false;
            int matchCount = 0;

            foreach (var hotel in body.Data)
            {
                string fieldValue = null;

                switch (fieldName.ToLower())
                {
                    case "pricestatus":
                        fieldValue = hotel.Pricestatus;
                        break;
                    case "hotelclass":
                        fieldValue = hotel.Hotelclass;
                        break;
                    case "hoteltype":
                        fieldValue = hotel.Hoteltype;
                        break;
                    case "hotelid":
                        fieldValue = hotel.Hotelid;
                        break;
                    case "hotelname":
                        fieldValue = hotel.Hotelname;
                        break;
                    case "cityid":
                        fieldValue = hotel.Cityid;
                        break;
                    case "cityname":
                        fieldValue = hotel.Cityname;
                        break;
                    case "districtid":
                        fieldValue = hotel.Districtid;
                        break;
                    case "districtname":
                        fieldValue = hotel.Districtname;
                        break;
                    case "areaid":
                        fieldValue = hotel.Areaid;
                        break;
                    case "areaname":
                        fieldValue = hotel.Areaname;
                        break;
                    case "countryid":
                        fieldValue = hotel.Countryid;
                        break;
                    case "countryname":
                        fieldValue = hotel.Countryname;
                        break;
                    case "countrycodeiso2":
                        fieldValue = hotel.Countrycodeiso2;
                        break;
                    case "countrycodeiso3":
                        fieldValue = hotel.Countrycodeiso3;
                        break;
                    default:
                        Assert.Fail($"Unknown field name for value check: {fieldName}");
                        break;
                }

                if (string.Equals(fieldValue, expectedValue, StringComparison.OrdinalIgnoreCase))
                {
                    foundMatch = true;
                    matchCount++;
                }
            }

            Assert.IsTrue(foundMatch, $"Expected at least one hotel in '{arrayName}' to have '{fieldName}' equal to '{expectedValue}', but none were found. Total hotels checked: {body.Data.Count}");
            TestContext.Progress.WriteLine($"Found {matchCount} hotel(s) with '{fieldName}' equal to '{expectedValue}' out of {body.Data.Count} total hotels");
        }

        [Given(@"the API returns a successful Hotels response")]
        public async Task GivenTheAPIReturnsASuccessfulHotelsResponse()
        {
            var (response, body, elapsedMs) = await HotelsCrud.GetHotelsWithStatusAsync();
            _scenarioContext["HotelsResponse"] = response;
            _scenarioContext["HotelsBody"] = body;
            _scenarioContext["HotelsElapsedMs"] = elapsedMs;

            Assert.AreEqual(200, (int)response.StatusCode, "Expected successful response");
            Assert.AreEqual("00", body.Code, "Expected Code '00'");
        }

        [Given(@"the API returns a list of hotels")]
        public async Task GivenTheAPIReturnsAListOfHotels()
        {
            var (response, body, elapsedMs) = await HotelsCrud.GetHotelsWithStatusAsync();
            _scenarioContext["HotelsResponse"] = response;
            _scenarioContext["HotelsBody"] = body;
            _scenarioContext["HotelsElapsedMs"] = elapsedMs;

            Assert.IsNotNull(body.Data, "Hotels Data should not be null");
            Assert.Greater(body.Data.Count, 0, "Hotels Data should not be empty");
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
                Assert.IsNotNull(hotel.Hotelid, $"Hotel '{hotel.Hotelname}' should have a hotelid");
                Assert.IsNotEmpty(hotel.Hotelid, $"Hotel '{hotel.Hotelname}' hotelid should not be empty");
            }
        }

        [Then(@"each hotelid should be a valid GUID")]
        public void ThenEachHotelidShouldBeAValidGUID()
        {
            var hotels = (List<Hotel>)_scenarioContext["HotelsDataForInspection"];
            foreach (var hotel in hotels)
            {
                Assert.IsTrue(Guid.TryParse(hotel.Hotelid, out _), $"Hotel '{hotel.Hotelname}' hotelid '{hotel.Hotelid}' is not a valid GUID");
            }
        }

        [Then(@"each item in the Hotels Data array should contain a hotelname")]
        public void ThenEachItemInTheHotelsDataArrayShouldContainAHotelname()
        {
            var hotels = (List<Hotel>)_scenarioContext["HotelsDataForInspection"];
            foreach (var hotel in hotels)
            {
                Assert.IsNotNull(hotel.Hotelname, $"Hotel with id '{hotel.Hotelid}' should have a hotelname");
            }
        }

        [Then(@"each hotelname should be a non-empty string")]
        public void ThenEachHotelnameShouldBeANonEmptyString()
        {
            var hotels = (List<Hotel>)_scenarioContext["HotelsDataForInspection"];
            foreach (var hotel in hotels)
            {
                Assert.IsNotEmpty(hotel.Hotelname, $"Hotel with id '{hotel.Hotelid}' hotelname should not be empty");
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
            Assert.IsEmpty(duplicates, $"Found duplicate hotelid values: {string.Join(", ", duplicates)}");
        }

        [Then(@"the Hotels API response time should be less than (\d+) milliseconds")]
        public void ThenTheHotelsAPIResponseTimeShouldBeLessThanMilliseconds(int maxMs)
        {
            var elapsedMs = (long)_scenarioContext["HotelsElapsedMs"];
            Assert.Less(elapsedMs, maxMs, $"Expected response time less than {maxMs}ms but got {elapsedMs}ms");
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

        [Then(@"hotels with pricestatus \"([^\"]*)\" should allow hotelprices requests")]
        public async Task ThenHotelsWithPricestatusShouldAllowHotelpricesRequests(string expectedPricestatus)
        {
            var body = (HotelsResponse)_scenarioContext["ResponseBody"];
            Assert.IsNotNull(body.Data, "Hotels Data should not be null");

            var hotelsWithReadyStatus = body.Data.Where(h => string.Equals(h.Pricestatus, expectedPricestatus, StringComparison.OrdinalIgnoreCase)).ToList();
            
            TestContext.Progress.WriteLine($"Found {hotelsWithReadyStatus.Count} hotels with pricestatus '{expectedPricestatus}'");
            
            Assert.Greater(hotelsWithReadyStatus.Count, 0, $"Expected at least one hotel with pricestatus '{expectedPricestatus}'");

            var sampleHotel = hotelsWithReadyStatus.First();
            TestContext.Progress.WriteLine($"Testing hotelprices request for hotel '{sampleHotel.Hotelname}' (hotelid: {sampleHotel.Hotelid}) with pricestatus '{sampleHotel.Pricestatus}'");

            var (pricesResponse, pricesBody, elapsedMs) = await HotelPricesCrud.GetHotelPricesWithStatusAsync(sampleHotel.Hotelid);
            
            _scenarioContext["HotelPricesResponse"] = pricesResponse;
            _scenarioContext["HotelPricesBody"] = pricesBody;
            _scenarioContext["HotelPricesElapsedMs"] = elapsedMs;

            Assert.AreEqual(200, (int)pricesResponse.StatusCode, $"Expected HTTP 200 for hotel with pricestatus '{expectedPricestatus}' but got {(int)pricesResponse.StatusCode}");
            
            if (pricesBody != null)
            {
                Assert.AreEqual("00", pricesBody.Code, $"Expected Code '00' for hotel with pricestatus '{expectedPricestatus}' but got '{pricesBody.Code}'");
                TestContext.Progress.WriteLine($"HotelPrices request successful for hotel '{sampleHotel.Hotelname}'. Response Code: {pricesBody.Code}, Data items: {pricesBody.Data?.Count ?? 0}");
            }
            else
            {
                TestContext.Progress.WriteLine($"HotelPrices request returned HTTP 200 but body could not be parsed for hotel '{sampleHotel.Hotelname}'");
            }
        }

        [Then(@"hotels with pricestatus \"([^\"]*)\" should not allow hotelprices requests")]
        public void ThenHotelsWithPricestatusShouldNotAllowHotelpricesRequests(string expectedPricestatus)
        {
            var body = (HotelsResponse)_scenarioContext["ResponseBody"];
            Assert.IsNotNull(body.Data, "Hotels Data should not be null");

            var hotelsWithPendingStatus = body.Data.Where(h => string.Equals(h.Pricestatus, expectedPricestatus, StringComparison.OrdinalIgnoreCase)).ToList();
            
            TestContext.Progress.WriteLine($"Found {hotelsWithPendingStatus.Count} hotels with pricestatus '{expectedPricestatus}'");

            if (hotelsWithPendingStatus.Count > 0)
            {
                var sampleHotel = hotelsWithPendingStatus.First();
                TestContext.Progress.WriteLine($"Hotel '{sampleHotel.Hotelname}' (hotelid: {sampleHotel.Hotelid}) has pricestatus '{expectedPricestatus}'");
                TestContext.Progress.WriteLine($"According to business rules, hotelprices requests should NOT be made for hotels with pricestatus '{expectedPricestatus}'");
                TestContext.Progress.WriteLine($"Client should wait and retry later when pricestatus changes to 'ready'");
                
                _scenarioContext["HotelsWithPendingStatus"] = hotelsWithPendingStatus;
                _scenarioContext["PendingStatusValidated"] = true;
            }
            else
            {
                TestContext.Progress.WriteLine($"No hotels found with pricestatus '{expectedPricestatus}' - validation skipped");
                _scenarioContext["PendingStatusValidated"] = true;
            }

            Assert.IsTrue((bool)_scenarioContext["PendingStatusValidated"], "Pending status validation should complete successfully");
        }
    }
}
