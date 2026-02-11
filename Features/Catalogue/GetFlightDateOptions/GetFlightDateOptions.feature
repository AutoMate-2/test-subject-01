@tointegration
Feature: Catalogue/GetFlightDateOptions

  As a consumer of Catalogue API
  I want to retrieve available flight dates
  So users can select valid travel dates


@tointegration_smoke
Scenario: TC-TO-CAT-030 - Get flight date options for future month
  Given I am authenticated with valid token
  Then I request flight date options for a future month
  Then the response status code should be 200
  And the response should contain "availableDates" array
  And each date should have:
    | Field            |
    | departureDate    |
    | remainingTickets |


@tointegration_smoke
Scenario: TC-TO-CAT-031 - Get flight date options without required parameters
  Given I am authenticated with valid token
  When I send a GET request to "/api/Catalogue/GetFlightDateOptions"
  Then the response status code should be 400


Scenario: TC-TO-CAT-032 - Verify remainingTicketLimit in response
  Given I am authenticated with valid token
  Then I request flight date options for a future month
  Then the response status code should be 200
  And the response for flightoptions should contain "remainingTicketLimit"
  And "remainingTicketLimit" should be a positive integer


Scenario: TC-TO-CAT-033 - Verify flight dates are within requested month
  Given I am authenticated with valid token
  Then I request flight date options for a specific month
  Then the response status code should be 200
  And all "departureDate" values should belong to the requested month and year


Scenario: TC-TO-CAT-034 - Get flight date options with invalid CityUID
  Given I am authenticated with valid token
  When I request flight date options with an invalid cityUID
  Then the response status code should be 400


Scenario: TC-TO-CAT-035 - Get flight date options with past date
  Given I am authenticated with valid token
  Then I request flight date options for a past month
  Then the response status code should be 200
  And the "availableDates" array should be empty
