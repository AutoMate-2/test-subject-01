@tointegration
Feature: Catalogue/GetDepartureCities - CL-T4

@smoke @api @catalogue @positive
Scenario: CL-T4-012 - Get departure cities for Kazakhstan
  Given I am authenticated with valid token
  When I send a GET request to "/api/Catalogue/GetDepartureCities?countryCode=KZ"
  Then the response status code should be 200
  And the response should be an array
  And each city should have required fields:
    | Field          |
    | cityUID        |
    | cityName       |
    | countryCode    |
    | cityID         |
    | countryID      |
    | timeZoneOffset |

@smoke @api @catalogue @negative
Scenario: CL-T4-013 - Get departure cities without country code
  Given I am authenticated with valid token
  When I send a GET request to "/api/Catalogue/GetDepartureCities"
  Then the response status code should be 400

@regression @api @catalogue @positive
Scenario: CL-T4-014 - Verify Kazakhstan departure cities include major cities
  Given I am authenticated with valid token
  When I send a GET request to "/api/Catalogue/GetDepartureCities?countryCode=KZ"
  Then the response status code should be 200
  And the response should contain city "Almaty"
  And the response should contain city "Astana"
  And the response should contain city "Aktau"

@regression @api @catalogue @negative
Scenario: CL-T4-015 - Get departure cities with invalid country code
  Given I am authenticated with valid token
  When I send a GET request to "/api/Catalogue/GetDepartureCities?countryCode=XX"
  Then the response status code should be 200
  And the response should be an empty array

@regression @api @catalogue @boundary
Scenario Outline: CL-T4-016 - Get departure cities with boundary country codes
  Given I am authenticated with valid token
  When I send a GET request to "/api/Catalogue/GetDepartureCities?countryCode=<countryCode>"
  Then the response status code should be <expectedStatus>

  Examples:
    | countryCode | expectedStatus |
    | KZ          | 200            |
    | AE          | 200            |
    | A           | 400            |
    | ABC         | 400            |
    | 12          | 400            |
    |             | 400            |

@regression @api @catalogue @positive
Scenario: CL-T4-017 - Verify cityUID is valid UUID format
  Given I am authenticated with valid token
  When I send a GET request to "/api/Catalogue/GetDepartureCities?countryCode=KZ"
  Then the response status code should be 200
  And each cityUID should be a valid UUID format for KZ

@regression @api @catalogue @positive
Scenario: CL-T4-018 - Verify timeZoneOffset values
  Given I am authenticated with valid token
  When I send a GET request to "/api/Catalogue/GetDepartureCities?countryCode=KZ"
  Then the response status code should be 200
  And each timeZoneOffset should be a number between -12 and 14 for KZ
