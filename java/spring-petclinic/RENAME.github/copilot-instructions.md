# Spring PetClinic - Coding Instructions

## Project Overview
This is a Spring Boot 3.5 MVC application demonstrating modern Spring Framework patterns with JPA, Thymeleaf, and clean architecture principles. 

## Architecture & Technologies
- **Framework**: Spring Boot 3.5 with Spring MVC
- **Java Version**: Java 17+
- **Database**: H2 (default), MySQL, PostgreSQL support
- **ORM**: Spring Data JPA with Hibernate
- **Template Engine**: Thymeleaf
- **Testing**: JUnit 5, Mockito, Spring Test, MockMvc
- **Build Tool**: Maven with Maven Wrapper
- **Containerization**: Docker support

## Project Structure
```
spring-petclinic/
├── README.md                           # Project documentation and quick start guide
├── docker-compose.yml                  # Docker containerization setup
├── Dockerfile                          # Container build instructions
├── pom.xml                             # Maven dependencies and build configuration
├── mvnw, mvnw.cmd                     # Maven wrapper scripts
├── .env.example, .env.docker          # Environment configuration templates
└── src/
    ├── main/
    │   ├── java/org/springframework/samples/petclinic/
    │   │   ├── PetClinicApplication.java   # Spring Boot main class
    │   │   ├── owner/                      # Owner domain (controllers, models, repositories)
    │   │   ├── vet/                        # Veterinarian domain
    │   │   ├── model/                      # Base entities (BaseEntity, Person, NamedEntity)
    │   │   ├── system/                     # System controllers (welcome, error)
    │   │   └── api/                        # API configuration
    │   ├── resources/                      # Application properties, static files
    │   └── scss/                          # SASS stylesheets
    └── test/
        ├── java/                          # Unit and integration tests
        └── jmeter/                        # Performance tests
```

## Naming Conventions & Patterns

### Controllers
- **MVC Controllers**: `[Entity]Controller` (e.g., `OwnerController`)
- Package by feature: controllers live in domain packages (`owner/`, `vet/`)
- Use `@Controller` annotation for MVC controllers
- Follow Spring MVC conventions with `@GetMapping`, `@PostMapping`
- Inject repositories via constructor injection

### Models
- Inherit from `BaseEntity` (for ID) or `Person`/`NamedEntity` as appropriate
- Use JPA annotations (`@Entity`, `@Table`, `@Column`)
- Use Bean Validation annotations (`@NotBlank`, `@Pattern`, etc.)
- Follow JavaBean conventions with proper getters/setters
- Include comprehensive Javadoc comments

### Repositories
- Extend `JpaRepository<Entity, ID>` for basic CRUD operations
- Use Spring Data JPA query methods (method name conventions)
- Custom queries with `@Query` annotation when needed
- Package repositories with their domain entities

### Testing
- **Unit Tests**: Use `@WebMvcTest` for controller testing with MockMvc
- **Integration Tests**: Use `@SpringBootTest` for full application context
- Use `@MockitoBean` for mocking repository dependencies
- Test class naming: `[Entity]ControllerTests` for unit tests
- Include test data setup methods (e.g., test fixtures)
- Use Hamcrest matchers for assertions (`hasProperty`, `hasSize`, etc.)

## Best Practices

### Code Style
- Follow Spring Framework coding conventions
- Use constructor injection for dependencies
- Implement proper exception handling
- Include comprehensive Javadoc documentation
- Use meaningful variable names and constants

### Error Handling
- Use Spring MVC error handling mechanisms
- Validate user input with Bean Validation
- Use `BindingResult` for form validation errors
- Implement custom error pages

### Database
- Use Spring Data JPA conventions
- Configure multiple database profiles (H2, MySQL, PostgreSQL)
- Use `@Transactional` appropriately
- Initialize data with SQL scripts in resources

### Security
- Follow secure coding practices
- Validate all user inputs
- Use proper CSRF protection (Spring Security when applicable)

## Development Workflow

### Test-Driven Development Approach
1. **Start with Tests**: Always begin feature development by proposing test cases to the user
   - Unit tests for controller behavior using MockMvc
   - Integration tests for full application workflows
   - Include test data setup that matches existing patterns
2. **Create Implementation**: Write the minimal code needed to make tests pass
3. **Run Tests**: Validate implementation with `./mvnw test`
4. **Refactor**: Improve code quality while keeping tests green

### Development Commands
- Run application: `./mvnw spring-boot:run`
- Run tests: `./mvnw test`
- Build: `./mvnw package`
- Docker: `docker-compose up` (from root directory)
- VS Code: Use provided tasks and launch configurations

### Feature Development Process
1. **Propose test scenarios** covering happy path, edge cases, and error conditions
2. **Get user approval** on test approach and coverage
3. **Implement failing tests** following existing patterns in `src/test/java/`
4. **Create minimal implementation** to satisfy tests
5. **Run full test suite** to ensure no regressions
6. **Validate functionality** matches Spring Framework best practices
