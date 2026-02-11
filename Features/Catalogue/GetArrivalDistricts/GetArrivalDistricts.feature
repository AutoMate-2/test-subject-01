@tointegration
Feature: Catalogue/GetArrivalDistricts


@tointegration_smoke
Scenario: TC-TO-CAT-019 - Arrival districts endpoint should work for all configured country codes
  Given I am authenticated with valid token
  Then arrival districts response should be valid for all configured country codes


Scenario: TC-TO-CAT-020 - Verify arrival districts structure for all configured country codes
  Given I am authenticated with valid token
  Then arrival districts structure should be valid for all configured country codes


Scenario: TC-TO-CAT-021 - Verify district parent references city for all configured country codes
  Given I am authenticated with valid token
  Then each district parent should reference a valid city for all configured country codes


Scenario: TC-TO-CAT-022 - Verify UAE includes major cities
  Given I am authenticated with valid token
  When I send a GET request to "/api/Catalogue/GetArrivalDistricts?countryCode=AE"
  Then the response status code should be 200
  And the arrival districts response should contain city "Dubai"
  And the arrival districts response should contain city "Abu Dhabi"
  And the arrival districts response should contain city "Sharjah"


Scenario: TC-TO-CAT-023 - Get arrival districts with invalid country code
  Given I am authenticated with valid token
  When I send a GET request to "/api/Catalogue/GetArrivalDistricts?countryCode=INVALID"
  Then the response status code should be 400
