using TechTalk.SpecFlow;
using to_integrations.CRUD.Catalogue.Api;
using to_integrations.CRUD.Catalogue;
using to_integrations.HelperMethods;

namespace to_integrations.Features.Common.Steps
{
    [Binding]
    public class CommonApiSteps
    {
        private readonly GetCountriesCrud _getCountriesCrud;
        private readonly CatalogueCrud _destinationCountriesCrud;
        private readonly ScenarioContext _scenarioContext;
        private readonly GetDepartureCitiesCrud _departureCitiesCrud;
        private readonly GetArrivalDistrictsCrud _getArrivalDistrictsCrud;
        private readonly GetHotelsCrud _getHotelsCrud;
        private readonly GetFlightDateOptionsCrud _getFlightDateOptionsCrud;

        public CommonApiSteps(
            GetCountriesCrud getCountriesCrud,
            CatalogueCrud destinationCountriesCrud,
            GetDepartureCitiesCrud departureCitiesCrud,
             GetArrivalDistrictsCrud getArrivalDistrictsCrud,
             GetHotelsCrud getHotelsCrud,
             GetFlightDateOptionsCrud getFlightDateOptionsCrud,
            ScenarioContext scenarioContext)
        {
            _getCountriesCrud = getCountriesCrud;
            _destinationCountriesCrud = destinationCountriesCrud;
            _departureCitiesCrud = departureCitiesCrud;
            _getArrivalDistrictsCrud = getArrivalDistrictsCrud;
            _getHotelsCrud = getHotelsCrud;
            _getFlightDateOptionsCrud = getFlightDateOptionsCrud;
            _scenarioContext = scenarioContext;
        }


        [When(@"I send a GET request to ""(.*)""")]
        public async Task WhenISendAGetRequestTo(string endpoint)
        {
            var token = _scenarioContext.ContainsKey("NoAuth")
     ? string.Empty
     : TokenCache.CachedToken;


            if (endpoint.Contains("GetCountries"))
            {
                await _getCountriesCrud.CallGetCountriesApi(token);
                _scenarioContext.Set(_getCountriesCrud.LastStatus, "LastStatus");
                return;
            }

            if (endpoint.Contains("GetDestinationCountries"))
            {
                await _destinationCountriesCrud.GetDestinationCountriesAsync("", token);
                _scenarioContext.Set(_destinationCountriesCrud.LastStatus, "LastStatus");
                return;
            }

            if (endpoint.Contains("GetDepartureCities"))
            {
                var countryCode = "";

                if (endpoint.Contains("countryCode="))
                    countryCode = endpoint.Split("countryCode=").Last();

                await _departureCitiesCrud.CallGetDepartureCities(countryCode, token);
                _scenarioContext.Set(_departureCitiesCrud.LastStatus, "LastStatus");
                return;
            }

            if (endpoint.Contains("GetArrivalDistricts"))
            {
                // extract countryCode if present
                var countryCode = "";

                if (endpoint.Contains("countryCode="))
                    countryCode = endpoint.Split("countryCode=")[1];

                await _getArrivalDistrictsCrud.CallAsync(countryCode, TokenCache.CachedToken);
                _scenarioContext.Set(_getArrivalDistrictsCrud.LastStatus, "LastStatus");
                return;
            }

            if (endpoint.Contains("GetHotels"))
            {
                var countryCode = endpoint.Contains("countryCode=")
                    ? endpoint.Split("countryCode=")[1]
                    : "";

                await _getHotelsCrud.CallAsync(countryCode, TokenCache.CachedToken);
                _scenarioContext.Set(_getHotelsCrud.LastStatus, "LastStatus");
                return;
            }

            if (endpoint.Contains("GetFlightDateOptions"))
            {
                // No parameters case (TC-TO-CAT-031)
                if (!endpoint.Contains("?"))
                {
                    await _getFlightDateOptionsCrud.CallAsync(
                        cityUid: string.Empty,
                        countryCode: string.Empty,
                        isFrom: true,
                        month: 0,
                        year: 0,
                        token: TokenCache.CachedToken
                    );

                    _scenarioContext.Set(_getFlightDateOptionsCrud.LastStatus, "LastStatus");
                    return;
                }

                // Other cases are handled by dedicated Steps (future/past/invalid city)
                throw new Exception(
                    "GetFlightDateOptions with parameters should be called via dedicated steps, not CommonApiSteps");
            }


            throw new Exception($"Unsupported endpoint: {endpoint}");
        }
    }

}
