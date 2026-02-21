# Guide: Building Solid and Working Test Cases in .NET (Reqnroll/NUnit)

## 1. Test Case Logic
- **Arrange**: Set up all required data, mocks, and environment.
- **Act**: Execute the action or API call under test.
- **Assert**: Validate the outcome (status code, response body, business rules).

### Example Structure
```csharp
[Test]
public void ShouldReturnSuccessForValidRequest()
{
    // Arrange
    var request = new RequestModel { ... };
    var expectedCode = "00";

    // Act
    var response = api.SendRequest(request);

    // Assert
    Assert.AreEqual(200, response.StatusCode);
    Assert.AreEqual(expectedCode, response.Body.Code);
}
```

## 2. Syntax for Step Definitions (Reqnroll)
- Use `[Given]`, `[When]`, `[Then]` attributes.
- Match step text exactly to feature file.
- Prefer explicit types and clear parameter names.

### Example
```csharp
[Given("I have valid authentication credentials")]
public void GivenIHaveValidAuthenticationCredentials() { ... }

[When("I send a GET request to the Cities endpoint")]
public async Task WhenISendAGETRequestToTheCitiesEndpoint() { ... }

[Then("the response status code should be {int}")]
public void ThenTheResponseStatusCodeShouldBe(int expectedStatusCode) { ... }
```

## 3. Clean Code Principles
- **Single Responsibility**: Each test should verify one behavior.
- **Descriptive Names**: Use clear method and variable names.
- **No Magic Strings**: Use constants or config for repeated values.
- **Minimal Setup**: Only set up what is needed for the test.
- **No Side Effects**: Tests should not depend on or modify global state.

## 4. Working Code Patterns
- Use `ScenarioContext` to share data between steps.
- Always check for nulls before accessing response properties.
- Prefer async/await for API calls.
- Use `Assert` for all validations.

### Example Step Definition
```csharp
[Then("the response body Code should be {string}")]
public void ThenTheResponseBodyCodeShouldBe(string expectedCode)
{
    var body = _scenarioContext["CitiesBody"] as CitiesResponse;
    Assert.IsNotNull(body, "Response body was not captured or failed to deserialize");
    Assert.AreEqual(expectedCode, body.Code, $"Expected Code '{expectedCode}' but got '{body.Code}'");
}
```

## 5. Common Test Case Mistakes
- **Ambiguous Step Definitions**: Avoid generic steps that match multiple scenarios.
- **Missing Step Definitions**: Ensure every step in feature files has a matching method.
- **Improper Assertions**: Always assert expected values, not just existence.
- **Ignoring Errors**: Handle exceptions and failed deserialization gracefully.

## 6. Best Practices
- Keep tests isolated and repeatable.
- Use mocks for external dependencies.
- Run tests frequently and fix failures immediately.
- Refactor tests for clarity and maintainability.

---

**Summary:**
Solid test cases require clear logic, precise syntax, robust assertions, and clean code. Always match step definitions to feature files, validate all outcomes, and keep tests maintainable.
