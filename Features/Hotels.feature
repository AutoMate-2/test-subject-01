Feature: Hotels API
  As an API consumer
  I want to retrieve the list of available hotels
  So that I can use them in the booking flow

  Background:
    Given I have valid authentication credentials

  @CL-T42 @referential-integrity
  Scenario: Hotels areaid must reference a valid area
    Given I send a GET request to "/Areas"
    And I store all "areaid" values as validAreaIds
    When I send a GET request to "/Hotels"
    Then for each hotel in response "Data"
    And each hotel areaid should exist in the valid area IDs list
