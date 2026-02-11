@tointegration
Feature: Catalogue/GetDestinationCountries
  Listing only the countries where we sell products
  Where can I send tourists?

  @tointegration_smoke
  Scenario: TC-TO-CAT-001 - Get destination countries for hotels only
    Given I am authenticated with valid token
    When I send a GET request to "/api/Catalogue/GetDestinationCountries?onlyHotels=true"
    Then the response status code should be 200
   And the destination countries response should contain "countries" array
    And the response "error" should be false
    And each country should have required fields:
      | Field        |
      | countryID    |
      | countryCode  |
      | countryName  |
      | isO2         |
      | isO3         |
      | status       |
      | sellCurrency |

  @tointegration_smoke
  Scenario: TC-TO-CAT-002 - Get destination countries for hotels and flights
    Given I am authenticated with valid token
    When I send a GET request to "/api/Catalogue/GetDestinationCountries?onlyHotels=false"
    Then the response status code should be 200
    And the destination countries response should contain "countries" array
    And the response "error" should be false
    And the response should contain countries with flight packages

  @tointegration_smoke
  Scenario: TC-TO-CAT-003 - Get destination countries without authentication
    Given I am not authenticated
    When I send a GET request to "/api/Catalogue/GetDestinationCountries?onlyHotels=true"
    Then the response status code should be 401

  Scenario: TC-TO-CAT-004 - Get destination countries with invalid onlyHotels parameter
    Given I am authenticated with valid token
    When I send a GET request to "/api/Catalogue/GetDestinationCountries?onlyHotels=invalid"
    Then the response status code should be 400

  Scenario: TC-TO-CAT-005 - Verify country data format
    Given I am authenticated with valid token
    When I send a GET request to "/api/Catalogue/GetDestinationCountries?onlyHotels=true"
    Then the response status code should be 200
    And each "countryCode" should be 2 characters (ISO 3166-1 alpha-2)
    And each "isO3" should be 3 characters (ISO 3166-1 alpha-3)
    And each "sellCurrency" should be 3 characters (ISO 4217)
    And each "status" should be an integer
    And each "sortingOrder" should be an integer

  Scenario: TC-TO-CAT-006 - Verify difference between onlyHotels true and false
    Given I am authenticated with valid token
    When I get destination countries with onlyHotels=true
    And I get destination countries with onlyHotels=false
    Then the onlyHotels=false response should include all countries from onlyHotels=true
    And onlyHotels=false may include additional countries with flight packages

  Scenario: TC-TO-CAT-007 - Get destination countries with expired token
    Given I have an expired access token
    When I send a GET request to "/api/Catalogue/GetDestinationCountries?onlyHotels=true"
    Then the response status code should be 401

  Scenario: TC-TO-CAT-008 - Verify activityId in response
    Given I am authenticated with valid token
    When I send a GET request to "/api/Catalogue/GetDestinationCountries?onlyHotels=true"
    Then the response status code should be 200
    And the response should contain "activityId" for tracing
