---
name: generate-unit-tests
description: Generate XUnit unit tests for ASP.NET Core controllers and services using Moq. Use this skill when asked to write, create, or add unit tests to the PetClinic project.
---

# XUnit Test Generator for ASP.NET Core PetClinic

Generate unit tests for this project following the established patterns in `PetClinic/Tests/Unit/`.

## Test Framework and Libraries

- **Test framework**: XUnit (`[Fact]`, `[Theory]`, `[InlineData]`)
- **Mocking**: Moq (`Mock<T>`, `.Setup()`, `.ReturnsAsync()`, `.Verify()`)
- **Assertions**: XUnit built-ins (`Assert.IsType<T>`, `Assert.Equal`, `Assert.NotNull`, `Assert.True/False`)
- **Namespace**: `PetClinic.Tests.Unit`

## File Location

Place new test files in `PetClinic/Tests/Unit/`. Name them `[Entity]ControllerTests.cs`.

## Class Structure

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PetClinic.Controllers;
using PetClinic.Data.Repositories;
using PetClinic.Models;
using Xunit;

namespace PetClinic.Tests.Unit
{
    /// <summary>
    /// Unit tests for [Entity]Controller
    /// </summary>
    public class [Entity]ControllerTests
    {
        private const int TEST_[ENTITY]_ID = 1;

        private readonly Mock<I[Entity]Repository> _mock[Entity]Repository;
        private readonly Mock<ILogger<[Entity]Controller>> _mockLogger;
        private readonly [Entity]Controller _controller;

        public [Entity]ControllerTests()
        {
            _mock[Entity]Repository = new Mock<I[Entity]Repository>();
            _mockLogger = new Mock<ILogger<[Entity]Controller>>();
            _controller = new [Entity]Controller(
                _mock[Entity]Repository.Object,
                _mockLogger.Object);
        }

        /// <summary>
        /// Factory method returning a valid test entity instance
        /// </summary>
        private [Entity] Create[Name]() => new [Entity]
        {
            Id = TEST_[ENTITY]_ID,
            // ... required properties
        };
    }
}
```

## Test Method Patterns

### Arrange-Act-Assert (AAA)

Every test must follow the three-section AAA pattern with comments:

```csharp
[Fact]
public async Task MethodName_ExpectedBehavior_WhenCondition()
{
    // Arrange
    var entity = Create[Name]();
    _mock[Entity]Repository.Setup(r => r.FindByIdAsync(TEST_[ENTITY]_ID))
        .ReturnsAsync(entity);

    // Act
    var result = await _controller.Details(TEST_[ENTITY]_ID);

    // Assert
    var viewResult = Assert.IsType<ViewResult>(result);
    Assert.IsType<[Entity]>(viewResult.Model);
}
```

### Test Naming Convention

Use the pattern: `MethodName_ExpectedResult_WhenCondition`

Examples from the existing test suite:
- `TestInitCreationForm` — synchronous, no condition needed
- `TestProcessCreationFormSuccess` — POST success path
- `TestProcessCreationFormHasErrors` — POST validation failure path
- `TestShowOwner` — GET details success

### Mock Setup Examples

**Repository returning an entity:**
```csharp
_mockRepo.Setup(r => r.FindByIdAsync(TEST_OWNER_ID))
    .ReturnsAsync(CreateGeorge());
```

**Repository saving and returning saved entity:**
```csharp
_mockRepo.Setup(r => r.SaveAsync(It.IsAny<Owner>()))
    .ReturnsAsync(savedOwner);
```

**Repository returning a list:**
```csharp
_mockRepo.Setup(r => r.FindAllAsync())
    .ReturnsAsync(new List<[Entity]> { Create[Name]() });
```

**Repository returning null (not found):**
```csharp
_mockRepo.Setup(r => r.FindByIdAsync(99))
    .ReturnsAsync((Owner?)null);
```

## Test Cases to Generate

For each controller, always cover these scenarios:

### GET (display form / details)
- Happy path: entity found, correct view returned, model populated
- Not found: repository returns null, `NotFound()` result returned

### POST (create / edit)
- Success: valid model, repository called, redirect returned
- Validation failure: `ModelState.AddModelError(...)`, form view returned, repository NOT called

### Factory Methods

Always provide a named factory method for the test entity:
- `CreateGeorge()` for Owner (Id=1, FirstName="George", LastName="Franklin")
- `CreateFido()` for Pet (Id=1, Name="Max")
- Use the constant `TEST_[ENTITY]_ID = 1` for the primary entity ID

## Simulating Model Validation Errors

```csharp
_controller.ModelState.AddModelError("FieldName", "The FieldName field is required.");
```

Call this before invoking the controller action to simulate failed model binding.

## Asserting Results

| Scenario | Assertion |
|---|---|
| Returns a view | `Assert.IsType<ViewResult>(result)` |
| Returns a redirect | `Assert.IsType<RedirectToActionResult>(result)` |
| Returns NotFound | `Assert.IsType<NotFoundResult>(result)` |
| View name matches | `Assert.Equal("ViewName", viewResult.ViewName)` |
| Model type matches | `Assert.IsType<Owner>(viewResult.Model)` |
| Redirect action matches | `Assert.Equal("Details", redirectResult.ActionName)` |
| Route value matches | `Assert.Equal(1, redirectResult.RouteValues!["id"])` |
| ModelState invalid | `Assert.False(_controller.ModelState.IsValid)` |
| ModelState has key | `Assert.True(_controller.ModelState.ContainsKey("Address"))` |

## Integration Test Location

For tests that need the full HTTP stack, use `PetClinic/Tests/Integration/` and `WebApplicationFactory<Program>`. Name them `[Feature]IntegrationTests.cs`.
