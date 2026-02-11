@tointegration
Feature: Catalogue/GetCountries


@tointegration_smoke
Scenario: TC-TO-CAT-009 - Get all countries
  Given I am authenticated with valid token
  When I send a GET request to "/api/Catalogue/GetCountries"
  Then the response status code should be 200
  And the response should contain "countries" array
  #And the response "error" should be false
  And response time should be under 500 ms


Scenario: TC-TO-CAT-010 - Verify countries list contains major countries
  Given I am authenticated with valid token
  When I send a GET request to "/api/Catalogue/GetCountries"
  Then the response status code should be 200
  And the response should contain country with code "AE" 
  And the response should contain country with code "KZ"
  And the response should contain country with code "TR" 


Scenario: TC-TO-CAT-011 - Verify phone codes are valid
  Given I am authenticated with valid token
  When I send a GET request to "/api/Catalogue/GetCountries"
  Then the response status code should be 200
  And each "phoneCode" should be a valid international dialing code
