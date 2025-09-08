---
mode: 'agent'
description: 'Generate comprehensive unit and integration tests for .NET PetClinic components'
tools: ['codebase', 'editFiles', 'fetch', 'findTestFiles', 'runCommands', 'search', 'searchResults', 'terminalLastCommand', 'terminalSelection'] 
---

# Test Generator

Generate comprehensive unit and integration tests for .NET PetClinic components following the established patterns.

## Test Requirements

- Unit tests: Use XUnit, Moq for mocking, follow existing `[Entity]ControllerTests` patterns
- Integration tests: Use WebApplicationFactory with in-memory database
- Include test data setup methods (e.g., `CreateGeorge()` for sample Owner)
- Match assertions and test scenarios from original Spring PetClinic tests
- Test both happy path and error conditions

## Input

Specify the component you want tests for:

- Controller class name
- Repository interface
- Model/Entity class
- Or paste existing code that needs test coverage

## Output

- Complete test class with proper setup and teardown
- Mock configurations for dependencies
- Test data builders and helper methods
- Comprehensive test coverage including edge cases
- Both positive and negative test scenarios

Ready to generate tests for your .NET PetClinic components!
