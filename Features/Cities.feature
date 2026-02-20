Feature: Cities API
  As an API consumer
  I want to retrieve the list of available cities
  So that I can use them in the booking flow

  Background:
    Given I have valid authentication credentials

  @CL-T25 @performance
  Scenario: Response time requirement
    When I send a GET request to the Cities endpoint
    Then the API response time should be less than 2000 milliseconds
