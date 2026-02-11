@tointegration
@Auth
Feature: Auth/Auth - Login API
  Authentication endpoint for TO Integration API
  Returns JWT accessToken valid for 30 days

# ============================================
# SMOKE TESTS
# ============================================

@tointegration_smoke
Scenario: TC-TO-AUTH-001 - Login with valid credentials
  Given I have valid login credentials
  When I send a POST request to Auth endpoint
  Then the response status code should be 200
  And the response should contain "accessToken"
  And the response should contain "refreshToken"
  And the response "passwordExpired" should be a boolean

@tointegration_smoke
Scenario: TC-TO-AUTH-002 - Login with invalid credentials
  Given I have invalid login credentials
  When I send a POST request to Auth endpoint
  Then the response status code should be 401
  And the response should contain error message

# ============================================
# REGRESSION TESTS
# ============================================


Scenario: TC-TO-AUTH-003 - Login with empty username
  Given I have login credentials with empty login
  When I send a POST request to Auth endpoint
  Then the response status code should be 400
  And the response should contain validation error for "login"


Scenario: TC-TO-AUTH-004 - Login with empty password
  Given I have login credentials with empty password
  When I send a POST request to Auth endpoint
  Then the response status code should be 400
  And the response should contain validation error for "password"


Scenario: TC-TO-AUTH-005 - Login with missing request body
  Given I have no request body
  When I send a POST request to Auth endpoint without body
  Then the response status code should be 400


Scenario: TC-TO-AUTH-006a - Login with single character login
  Given I have login credentials with single character login
  When I send a POST request to Auth endpoint
  Then the response status code should be 401


Scenario: TC-TO-AUTH-006b - Login with 255 character login
  Given I have login credentials with 255 character login
  When I send a POST request to Auth endpoint
  Then the response status code should be 400


Scenario: TC-TO-AUTH-006c - Login with single character password
  Given I have login credentials with single character password
  When I send a POST request to Auth endpoint
  Then the response status code should be 401


Scenario: TC-TO-AUTH-006d - Login with 255 character password
  Given I have login credentials with 255 character password
  When I send a POST request to Auth endpoint
  Then the response status code should be 401


Scenario: TC-TO-AUTH-006e - Login with SQL injection in login
  Given I have login credentials with SQL injection
  When I send a POST request to Auth endpoint
  Then the response status code should be 401


Scenario: TC-TO-AUTH-006f - Login with XSS script in login
  Given I have login credentials with XSS script
  When I send a POST request to Auth endpoint
  Then the response status code should be 401


Scenario: TC-TO-AUTH-007 - Verify access token expiration is 30 days
  Given I have valid login credentials
  When I send a POST request to Auth endpoint
  Then the response status code should be 200
  And the "accessToken" should be a valid JWT token
  And the token expiration should be approximately 43200 minutes


Scenario: TC-TO-AUTH-008 - Verify token does not contain sensitive data
  Given I have valid login credentials
  When I send a POST request to Auth endpoint
  Then the response status code should be 200
  And the JWT payload should not contain password
  And the JWT payload should not contain sensitive user data
