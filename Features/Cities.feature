Feature: Cities API
  As an API consumer
  I want to retrieve the list of available cities
  So that I can use them in the booking flow

  Background:
    Given I have valid authentication credentials

  @CL-T25 @smoke
  Scenario: Successful response is returned
    When I send a GET request to the Cities endpoint
    Then the response status code should be 200
    And the response body Code should be "00"
    And the response body Message should be empty

  @CL-T27 @smoke
  Scenario: Cities list is not empty
    When I send a GET request to the Cities endpoint
    Then the response Data array should exist
    And the Data array length should be greater than 0

  @CL-T28 @data-validation
  Scenario: Each city has valid structure
    Given the API returns a successful response
    When I inspect each item in the Data array
    Then each item should contain a cityid
    And each cityid should be a valid GUID
    And each item should contain a cityname
    And each cityname should be a non-empty string

  @CL-T29 @data-integrity
  Scenario: No duplicate city IDs exist
    Given the API returns a list of cities
    When I collect all cityid values
    Then there should be no duplicate cityid values

  @CL-T30 @performance
  Scenario: Response time under 2 seconds
    When I send a GET request to the Cities endpoint
    Then the API response time should be less than 2000 milliseconds
