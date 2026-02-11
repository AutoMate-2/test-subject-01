﻿using TechTalk.SpecFlow;
using to_integrations.CRUD.Catalogue;
using to_integrations.CRUD.Catalogue.Api;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using to_integrations.HelperMethods;

namespace to_integrations.Features.Catalogue.GetDestinationCountries.Steps
{
    [Binding]
    public class GetDestinationCountriesSteps
    {
        private readonly CatalogueCrud _crud;
        private readonly ScenarioContext _scenarioContext;
        private string _currentToken = "";
        private JArray? _hotelsOnlyResponse;
        private JArray? _allCountriesResponse;

        public GetDestinationCountriesSteps(CatalogueCrud crud, ScenarioContext scenarioContext)
        {
            _crud = crud;
            _scenarioContext = scenarioContext;
        }

        private void SetTestCaseFromScenario()
        {
            var scenarioTitle = _scenarioContext.ScenarioInfo.Title;
            _crud.SetTestCase(scenarioTitle);
        }

        [Given("I am not authenticated")]
        public void GivenIAmNotAuthenticated()
        {
            SetTestCaseFromScenario();
            _scenarioContext.Set(true, "NoAuth");
            _currentToken = "";
        }

        [Given("I have an expired access token")]
        public void GivenIHaveAnExpiredAccessToken()
        {
            SetTestCaseFromScenario();
            _scenarioContext.Set(true, "NoAuth");
            _currentToken = CataloguePresetup.ExpiredToken;
        }

        [When("I get destination countries with onlyHotels=true")]
        public async Task WhenIGetDestinationCountriesWithOnlyHotelsTrue()
        {
            await _crud.GetDestinationCountriesAsync("?onlyHotels=true", _currentToken);
            _scenarioContext.Set(_crud.LastStatus, "LastStatus");
            _hotelsOnlyResponse = _crud.GetCountries();
        }

        [When("I get destination countries with onlyHotels=false")]
        public async Task WhenIGetDestinationCountriesWithOnlyHotelsFalse()
        {
            await _crud.GetDestinationCountriesAsync("?onlyHotels=false", _currentToken);
            _scenarioContext.Set(_crud.LastStatus, "LastStatus");
            _allCountriesResponse = _crud.GetCountries();
        }

        [Then(@"the destination countries response should contain ""(.*)"" array")]
        public void ThenTheResponseShouldContainArray(string fieldName)
        {
            _crud.AssertResponseContainsArray(fieldName);
        }

        [Then("the response \"(.*)\" should be false")]
        public void ThenTheResponseFieldShouldBeFalse(string fieldName)
        {
            _crud.AssertResponseFieldIsFalse(fieldName);
        }

        [Then("each country should have required fields:")]
        public void ThenEachCountryShouldHaveRequiredFields(Table table)
        {
            var requiredFields = table.Rows.Select(row => row[0]).ToArray();
            _crud.AssertCountriesHaveRequiredFields(requiredFields);
        }

        [Then("the response should contain countries with flight packages")]
        public void ThenTheResponseShouldContainCountriesWithFlightPackages()
        {
            _crud.AssertResponseContainsCountriesWithFlights();
        }

        [Then("each \"(.*)\" should be 2 characters \\(ISO 3166-1 alpha-2\\)")]
        public void ThenEachCountryCodeShouldBe2Characters(string fieldName)
        {
            _crud.AssertCountryCodeFormat();
        }

        [Then("each \"(.*)\" should be 3 characters \\(ISO 3166-1 alpha-3\\)")]
        public void ThenEachISO3ShouldBe3Characters(string fieldName)
        {
            _crud.AssertCountryISO3Format();
        }

        [Then("each \"(.*)\" should be 3 characters \\(ISO 4217\\)")]
        public void ThenEachSellCurrencyShouldBe3Characters(string fieldName)
        {
            _crud.AssertSellCurrencyFormat();
        }

        [Then("each \"(.*)\" should be an integer")]
        public void ThenEachFieldShouldBeAnInteger(string fieldName)
        {
            if (fieldName == "status")
            {
                _crud.AssertStatusIsInteger();
            }
            else if (fieldName == "sortingOrder")
            {
                _crud.AssertSortingOrderIsInteger();
            }
        }

        [Then("the onlyHotels=false response should include all countries from onlyHotels=true")]
        public void ThenOnlyHotelsFalseResponseShouldIncludeAllCountriesFromOnlyHotelsTrue()
        {
            if (_hotelsOnlyResponse == null)
            {
                var report = PlainTextReporter.CreateReport(_scenarioContext.ScenarioInfo.Title);
                report.Assertion = "Hotels-only response capture";
                report.Expected = "onlyHotels=true response should be captured";
                report.Actual = "onlyHotels=true response not captured";
                PlainTextReporter.ThrowFailure(report);
            }

            if (_allCountriesResponse == null)
            {
                var report = PlainTextReporter.CreateReport(_scenarioContext.ScenarioInfo.Title);
                report.Assertion = "All countries response capture";
                report.Expected = "onlyHotels=false response should be captured";
                report.Actual = "onlyHotels=false response not captured";
                PlainTextReporter.ThrowFailure(report);
            }

            var parentIds = _allCountriesResponse!
                .Select(item => item["countryID"]?.ToString())
                .Where(id => id != null)
                .ToHashSet();

            var subsetIds = _hotelsOnlyResponse!
                .Select(item => item["countryID"]?.ToString())
                .Where(id => id != null)
                .ToList();

            var missingIds = subsetIds.Where(id => !parentIds.Contains(id)).ToList();

            if (missingIds.Count > 0)
            {
                var missing = string.Join(", ", missingIds.Take(5));
                var report = PlainTextReporter.CreateReport(_scenarioContext.ScenarioInfo.Title);
                report.Assertion = "Array subset validation";
                report.Expected = "All countries from onlyHotels=true should exist in onlyHotels=false";
                report.Actual = $"Found {missingIds.Count} countries in onlyHotels=true missing from onlyHotels=false: {missing}";

                var step = PlainTextReporter.CreateStep("When I compare onlyHotels=true and onlyHotels=false responses");
                step.ErrorMessage = $"Some countries are missing from onlyHotels=false response";
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }
        }

        [Then("onlyHotels=false may include additional countries with flight packages")]
        public void ThenOnlyHotelsFalseMayIncludeAdditionalCountriesWithFlightPackages()
        {
            if (_allCountriesResponse == null)
            {
                var report = PlainTextReporter.CreateReport(_scenarioContext.ScenarioInfo.Title);
                report.Assertion = "All countries response capture";
                report.Expected = "onlyHotels=false response should be captured";
                report.Actual = "onlyHotels=false response not captured";
                PlainTextReporter.ThrowFailure(report);
            }

            var hasFlightCountries = _allCountriesResponse!.Any(c => c["hasFlights"] != null && c["hasFlights"].Value<bool>());
            
            if (!hasFlightCountries)
            {
                var report = PlainTextReporter.CreateReport(_scenarioContext.ScenarioInfo.Title);
                report.Assertion = "Flight packages availability check";
                report.Expected = "onlyHotels=false response should contain countries with flight packages";
                report.Actual = $"No countries with flight packages found in {_allCountriesResponse!.Count} countries";

                var step = PlainTextReporter.CreateStep("Then onlyHotels=false may include additional countries with flight packages");
                step.ErrorMessage = "onlyHotels=false response does not contain countries with flight packages";
                report.BddSteps.Add(step);

                PlainTextReporter.ThrowFailure(report);
            }
        }

        [Then("the response should contain \"(.*)\" for tracing")]
        public void ThenTheResponseShouldContainActivityIdForTracing(string fieldName)
        {
            _crud.AssertResponseContainsActivityId();
        }
    }
}
