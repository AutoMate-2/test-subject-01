@tointegration
Feature: Catalogue/GetHotels

  As a consumer of Catalogue API
  I want to retrieve hotels
  So that hotel data is consistent across all countries


@tointegration_smoke
Scenario: TC-TO-CAT-024 - Get hotels for all configured country codes
  Given I am authenticated with valid token
  When I send a GET request to "/api/Catalogue/GetHotels?countryCode="
  Then the response status code should be 200
  And GetHotels response should be an array
  And each hotel should have required fields:
    | Field        |
    | hotelID      |
    | hotelCode    |
    | hotelName    |
    | hotelClass   |
    | cityUID      |
    | countryCode  |
    | districtUID  |
    | hotelType    |
    | latitude     |
    | longitude    |


Scenario: TC-TO-CAT-026 - Verify hotel data types and values for all countries
  Given I am authenticated with valid token
  When I send a GET request to "/api/Catalogue/GetHotels?countryCode="
  Then the response status code should be 200
  And each "hotelCode" should be a valid UUID
  And each "hotelClass" should be one of: "1", "2", "3", "4", "5", "6"
  And each "hotelType" should be one of: "CityHotel", "BeachHotel", "SecondLineBeach"


Scenario: TC-TO-CAT-027 - Verify hotel boolean amenities for all countries
  Given I am authenticated with valid token
  When I send a GET request to "/api/Catalogue/GetHotels?countryCode="
  Then the response status code should be 200
  And each "hasAlcohol" should be a boolean
  And each "hasFreeWifi" should be a boolean
  And each "hasMetro" should be a boolean
  And each "hasPool" should be a boolean
  And each "hasMall" should be a boolean


Scenario: TC-TO-CAT-028 - Verify hotel coordinates are valid for all countries
  Given I am authenticated with valid token
  When I send a GET request to "/api/Catalogue/GetHotels?countryCode"
  Then the response status code should be 200
  And each "latitude" should be a valid latitude (-90 to 90)
  And each "longitude" should be a valid longitude (-180 to 180)


Scenario: TC-TO-CAT-028A - Verify UAE hotel coordinates are within UAE bounds
  Given I am authenticated with valid token
  When I send a GET request to "/api/Catalogue/GetHotels?countryCode=AE"
  Then the response status code should be 200
  And UAE hotels should have latitude approximately between 22 and 27
  And UAE hotels should have longitude approximately between 51 and 57
