Feature: Districts API
  As an API consumer
  I want to retrieve the list of available districts
  So that I can use them for precise hotel location filtering

  Background:
    Given I have valid authentication credentials

  @CL-T26 @smoke
@CL-T38
  Scenario: Successful response is returned
    When I send a GET request to the Districts endpoint
    Then the Districts response status code should be 200
    And the Districts response body Code should be "00"
    And the Districts response body Message should be empty

  @CL-T31 @smoke
@CL-T38
  Scenario: Districts list is not empty
    When I send a GET request to the Districts endpoint
    Then the Districts response Data array should exist
    And the Districts Data array length should be greater than 0

  @CL-T32 @data-validation
@CL-T38
  Scenario: Each district has valid structure
    Given the API returns a successful Districts response
    When I inspect each item in the Districts Data array
    Then each item should contain a districtid
    And each districtid should be a valid GUID
    And each item should contain a districtname
    And each districtname should be a non-empty string
    And each item should contain a cityid
    And each district cityid should be a valid GUID

  @CL-T33 @data-integrity
@CL-T38
  Scenario: No duplicate district IDs exist
    Given the API returns a list of districts
    When I collect all districtid values
    Then there should be no duplicate districtid values

  @CL-T34 @performance
@CL-T38
  Scenario: Response time requirement
    When I send a GET request to the Districts endpoint
    Then the Districts API response time should be less than 2000 milliseconds

  @CL-T35 @referential-integrity
@CL-T38
  Scenario: All district cityids exist in Cities
    Given I retrieve all valid city IDs from the Cities endpoint
    When I send a GET request to the Districts endpoint
    Then every district cityid should exist in the valid city IDs list
