# PetClinic .NET Core - Coding Instructions

## Project### Testing

- **Unit Tests**: Use `Moq` for mocking dependencies, test controller logic in isolation
- **Integration Tests**: Use `WebApplicationFactory<Program>` with in-memory database
- Test class naming: `[Entity]ControllerTests` for unit, `[Feature]IntegrationTests` for integration
- Include test data setup methods (e.g., `CreateGeorge()` for sample Owner)

## Architecture & Technologies

- **Framework**: ASP.NET Core 8.0 MVC with Minimal APIs
- **Database**: Entity Framework Core with SQLite (development)
- **Logging**: Serilog
- **Testing**: XUnit with Moq for mocking
- **API Documentation**: Swagger/OpenAPI
- **Containerization**: Docker support

## Project Structure

```
pet-clinic/
├── README.md                       # Project documentation and quick start guide
├── docker-compose.yml              # Docker containerization setup
├── Dockerfile                      # Container build instructions
├── PetClinic.sln                   # Solution file
└── PetClinic/                      # Main application
    ├── Controllers/                # MVC Controllers
    │   └── Api/                    # API Controllers (marked with [ApiController])
    ├── Data/
    │   ├── Repositories/      # Repository interfaces and implementations
    │   └── PetClinicContext.cs
    ├── Models/                # Domain entities
    ├── Views/                 # Razor views
    ├── Tests/
    │   ├── Unit/             # Unit tests using Moq
    │   └── Integration/      # Integration tests using WebApplicationFactory
    ├── Validators/           # Custom validation attributes
    ├── PetClinic.csproj      # Project dependencies and configuration
    └── Program.cs            # Application entry point and DI setup
```

## Naming Conventions & Patterns

### Controllers

- **MVC Controllers**: `[Entity]Controller` (e.g., `OwnersController`)
- **API Controllers**: `[Entity]ApiController` in `Controllers/Api/` namespace, visible through Swagger
- Use dependency injection for repositories and logging
- API controllers must have `[ApiController]` and `[Route("api/[controller]")]` attributes

### Models

- Inherit from `BaseEntity` (for Id) or `Person`/`NamedEntity` as appropriate
- Use Data Annotations for validation (`[Required]`, `[RegularExpression]`, etc.)
- Use `[Table("tablename")]` and `[Column("columnname")]` for database mapping

### Repositories

- Follow Repository Pattern: `I[Entity]Repository` interface + `[Entity]Repository` implementation
- Use async/await for all database operations (`FindAllAsync`, `FindByIdAsync`, etc.)
- Include XML documentation comments
- Register in DI container in `Program.cs`

### Testing

- **Unit Tests**: Use `Moq` for mocking dependencies, test controller logic in isolation
- **Integration Tests**: Use `WebApplicationFactory<Program>` with in-memory database
- Test class naming: `[Entity]ControllerTests` for unit, `[Feature]IntegrationTests` for integration
- Include test data setup methods (e.g., `CreateGeorge()` for sample Owner)

## Best Practices

### Code Style

- Enable nullable reference types and handle nullability appropriately
- Use `async/await` for all I/O operations
- Include comprehensive XML documentation
- Use meaningful variable names and constants (e.g., `TEST_OWNER_ID = 1`)

### Error Handling

- Use proper HTTP status codes in API controllers
- Add model validation errors to `ModelState`
- Include proper exception handling and logging

### Database

- Use Entity Framework conventions and configurations
- Seed database using `ApplicationDbInitializer.SeedAsync()`
- Use migrations for schema changes

### Security

- Enable antiforgery tokens (`AutoValidateAntiforgeryTokenAttribute`)
- Validate all user inputs
- Follow secure coding practices

## Development Workflow

### Test-Driven Development Approach

1. **Start with Tests**: Begin feature development by proposing test cases to the user
   - Unit tests for business logic and controller behavior
   - Integration tests for end-to-end workflows
2. **Create Implementation**: Write the minimal code needed to make tests pass
3. **Run Tests**: Validate implementation with `dotnet test`
4. **Refactor**: Improve code quality while keeping tests green

### Development Commands

- Run from `PetClinic/` directory: `dotnet run`
- Tests: `dotnet test`
- Build: `dotnet build`
- Docker: `docker-compose up` (from root directory)
- Always ensure code builds successfully before committing

### Recommended Development Process

1. **Propose test scenarios** covering happy path, edge cases, and error conditions
2. **Get user approval** on test approach and coverage
3. **Implement failing tests** following existing patterns in `Tests/Unit/` and `Tests/Integration/`
4. **Create minimal implementation** to satisfy tests
5. **Run full test suite** to ensure no regressions
