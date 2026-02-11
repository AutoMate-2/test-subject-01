@tointegration
Feature: Catalogue/GetDepartureCities


@tointegration_smoke
Scenario: TC-TO-CAT-012 - Departure cities endpoint should work for all configured country codes
  Given I am authenticated with valid token
  Then departure cities response should be valid for all configured country codes


Scenario: TC-TO-CAT-014 - Verify cityUID format for all configured country codes
  Given I am authenticated with valid token
  Then each cityUID should be valid for all configured country codes


Scenario: TC-TO-CAT-015 - Verify timeZoneOffset range for all configured country codes
  Given I am authenticated with valid token
  Then each timeZoneOffset should be valid for all configured country codes


Scenario: TC-TO-CAT-016 - Get departure cities without country code
  Given I am authenticated with valid token
  When I send a GET request to "/api/Catalogue/GetDepartureCities"
  Then the response status code should be 400


Scenario Outline: TC-TO-CAT-017 - Get departure cities with invalid country code
  Given I am authenticated with valid token
  When I send a GET request to "/api/Catalogue/GetDepartureCities?countryCode=<countryCode>"
  Then the response status code should be 400

  Examples:
    | countryCode |
    | A           |
    | ABC         |
    | 12          |
