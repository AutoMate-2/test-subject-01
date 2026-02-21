Feature: Areas API
  As an API consumer
  I want to retrieve the list of available areas
  So that I can use them for regional grouping of hotels

  Background:
    Given I have valid authentication credentials

  @CL-T36 @smoke
  Scenario: Areas API returns successful response code
    When I send a GET request to the Areas endpoint
    Then the Areas response status code should be 200
    And the Areas response body Code should be "00"

  @CL-T37 @smoke
  Scenario: Areas list should not be empty
    When I send a GET request to the Areas endpoint
    Then the Areas response Data array should exist
    And the Areas Data array length should be greater than 0

  @CL-T38 @data-validation
  Scenario: Each area must have a non-empty areaname
    Given valid agent credentials
    When I send a GET request to "/Areas"
    Then for each item in response "Data"
    And the field "areaname" must not be null
    And the field "areaname" must not be empty

  @CL-T39 @data-integrity
  Scenario: areaid values must be unique
    Given valid agent credentials
    When I send a GET request to "/Areas"
    Then all "areaid" values in response "Data" must be unique

  @CL-T40 @performance
  Scenario: Areas API response time should be under 2 seconds
    Given valid agent credentials
    When I send a GET request to "/Areas"
    Then the response time must be less than 2000 milliseconds
