Feature: Districts API
  As an API consumer
  I want to retrieve the list of available districts
  So that I can use them for precise hotel location filtering

  Background:
    Given I have valid authentication credentials

  @CL-T26 @referential-integrity
  Scenario: All district cityids exist in Cities
    Given I retrieve all valid city IDs from the Cities endpoint
    When I send a GET request to the Districts endpoint
    Then every district cityid should exist in the valid city IDs list
