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

  @CL-T43 @smoke
  Scenario: Hotels API returns successful response code
    Given valid agent credentials
    When I send a GET request to "/hotels"
    Then the HTTP status code should be 200
    And the response field "Code" should be "00"

  @CL-T44 @smoke
  Scenario: Hotels list should not be empty
    Given valid agent credentials
    When I send a GET request to "/hotels"
    Then the response field "Data" should contain at least one item

  @CL-T45 @data-validation
  Scenario: Each hotel must have a valid GUID as hotelid
    Given valid agent credentials
    When I send a GET request to "/hotels"
    Then for each item in response "Data"
    And the field "hotelid" must match GUID format

  @CL-T46 @data-validation
  Scenario: Each hotel must have a non-empty hotelname
    Given valid agent credentials
    When I send a GET request to "/hotels"
    Then for each item in response "Data"
    And the field "hotelname" must not be null
    And the field "hotelname" must not be empty

  @CL-T47 @data-validation
  Scenario: Hotel pricestatus must be either "ready" or "pending"
    Given valid agent credentials
    When I send a GET request to "/hotels"
    Then for each item in response "Data"
    And the field "pricestatus" must be either "ready" or "pending"

  @CL-T48 @data-validation
  Scenario: Hotel class must match allowed values
    Given valid agent credentials
    When I send a GET request to "/hotels"
    Then for each item in response "Data"
    And the field "hotelclass" must be one of "*", "**", "***", "****", "*****", "Apartments"

  @CL-T49 @data-validation
  Scenario: Hotel type must match allowed values
    Given valid agent credentials
    When I send a GET request to "/hotels"
    Then for each item in response "Data"
    And the field "hoteltype" must be either "City" or "Beach"

  @CL-T50 @data-validation
  Scenario: Hotel boolean fields must be actual boolean values
    Given valid agent credentials
    When I send a GET request to "/hotels"
    Then for each item in response "Data"
    And fields "hasalcohol", "hasfreewifi", "hasmall", "hasmetro", "haspool", "popular", "recommended" must be boolean

  @CL-T51 @data-validation
  Scenario: Country ISO codes must have correct length
    Given valid agent credentials
    When I send a GET request to "/hotels"
    Then for each item in response "Data"
    And the field "countrycodeiso2" must have length 2
    And the field "countrycodeiso3" must have length 3

  @CL-T52 @data-integrity
  Scenario: hotelid values must be unique
    Given valid agent credentials
    When I send a GET request to "/hotels"
    Then all "hotelid" values in response "Data" must be unique

  @CL-T53 @data-validation
  Scenario: At least one hotel must have pricestatus "ready"
    Given valid agent credentials
    When I send a GET request to "/hotels"
    Then at least one hotel in response "Data" must have "pricestatus" equal to "ready"

  @CL-T54 @referential-integrity
  Scenario: Every Hotels.cityid must exist in Cities.cityid
    Given I send a GET request to "/Cities"
    And I store all "cityid" values as validCityIds
    When I send a GET request to "/hotels"
    Then for each hotel in response "Data"
    And hotel.cityid must exist in validCityIds

  @CL-T55 @referential-integrity
  Scenario: Every Hotels.districtid must exist in Districts.districtid
    Given I send a GET request to "/Districts"
    And I store all "districtid" values as validDistrictIds
    When I send a GET request to "/hotels"
    Then for each hotel in response "Data"
    And hotel.districtid must exist in validDistrictIds

  @CL-T56 @referential-integrity
  Scenario: Hotels.areaid must reference valid area if not null
    Given I send a GET request to "/Areas"
    And I store all "areaid" values as validAreaIds
    When I send a GET request to "/hotels"
    Then for each hotel in response "Data"
    And hotel.areaid must exist in validAreaIds if not null

  @CL-T57 @business-logic
  Scenario: HotelPrices endpoint must respect pricestatus
    Given valid agent credentials
    When I send a GET request to "/hotels"
    Then hotels with pricestatus "ready" should allow hotelprices requests
    And hotels with pricestatus "pending" should not allow hotelprices requests
