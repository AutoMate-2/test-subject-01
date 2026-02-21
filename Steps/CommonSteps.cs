using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using Reqnroll;
using to_integrations.CRUD.Areas;
using to_integrations.CRUD.Cities;
using to_integrations.CRUD.Districts;
using to_integrations.HelperMethods;
using to_integrations.Models;

namespace to_integrations.Steps
{
    [Binding]
    public class CommonSteps
    {
        private readonly ScenarioContext _scenarioContext;

        public CommonSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"valid agent credentials")]
        public void GivenValidAgentCredentials()
        {
            Assert.IsNotNull(TokenCache.CachedToken, "Authentication token should be cached");
            TestContext.Progress.WriteLine("Using cached authentication token");
        }

        [Given(@"I have valid authentication credentials")]
        public void GivenIHaveValidAuthenticationCredentials()
        {
            Assert.IsNotNull(TokenCache.CachedToken, "Authentication token should be cached");
            TestContext.Progress.WriteLine("Using cached authentication token");
        }

        [Given(@"I send a GET request to \"([^\"]*)\"")] 
        public async Task GivenISendAGETRequestTo(string endpoint)
        {
            if (endpoint.ToLower().Contains("areas"))
            {
                var (response, body, elapsedMs) = await AreasCrud.GetAreasWithStatusAsync();
                _scenarioContext["AreasResponse"] = response;
                _scenarioContext["AreasBody"] = body;
                _scenarioContext["AreasElapsedMs"] = elapsedMs;
                _scenarioContext["HttpResponse"] = response;
                _scenarioContext["ResponseBody"] = body;
            }
            else if (endpoint.ToLower().Contains("cities"))
            {
                var (response, body, elapsedMs) = await CitiesCrud.GetCitiesWithStatusAsync();
                _scenarioContext["CitiesResponse"] = response;
                _scenarioContext["CitiesBody"] = body;
                _scenarioContext["CitiesElapsedMs"] = elapsedMs;
                _scenarioContext["HttpResponse"] = response;
                _scenarioContext["ResponseBody"] = body;
            }
            else if (endpoint.ToLower().Contains("districts"))
            {
                var (response, body, elapsedMs) = await DistrictsCrud.GetDistrictsWithStatusAsync();
                _scenarioContext["DistrictsResponse"] = response;
                _scenarioContext["DistrictsBody"] = body;
                _scenarioContext["DistrictsElapsedMs"] = elapsedMs;
                _scenarioContext["HttpResponse"] = response;
                _scenarioContext["ResponseBody"] = body;
            }
        }

        [Given(@"I store all \"([^\"]*)\" values as validAreaIds")]
        public void GivenIStoreAllValuesAsValidAreaIds(string fieldName)
        {
            var body = (AreasResponse)_scenarioContext["AreasBody"];
            var validAreaIds = new HashSet<string>(body.Data.Select(a => a.Areaid));
            _scenarioContext["validAreaIds"] = validAreaIds;
            TestContext.Progress.WriteLine($"Stored {validAreaIds.Count} valid area IDs");
        }

        [Given(@"I store all \"([^\"]*)\" values as validCityIds")]
        public void GivenIStoreAllValuesAsValidCityIds(string fieldName)
        {
            var body = (CitiesResponse)_scenarioContext["CitiesBody"];
            var validCityIds = new HashSet<string>(body.Data.Select(c => c.Cityid));
            _scenarioContext["validCityIds"] = validCityIds;
            TestContext.Progress.WriteLine($"Stored {validCityIds.Count} valid city IDs");
        }

        [Given(@"I store all \"([^\"]*)\" values as validDistrictIds")]
        public void GivenIStoreAllValuesAsValidDistrictIds(string fieldName)
        {
            var body = (DistrictsResponse)_scenarioContext["DistrictsBody"];
            var validDistrictIds = new HashSet<string>(body.Data.Select(d => d.Districtid));
            _scenarioContext["validDistrictIds"] = validDistrictIds;
            TestContext.Progress.WriteLine($"Stored {validDistrictIds.Count} valid district IDs");
        }
    }
}
