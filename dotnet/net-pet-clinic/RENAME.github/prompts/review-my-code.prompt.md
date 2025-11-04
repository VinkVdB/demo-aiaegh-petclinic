---
mode: 'agent'
description: "Perform a comprehensive code review of the Spring PetClinic application focusing on Spring Framework best practices, clean architecture, and maintainability."
tools: ['runCommands', 'runTasks', 'extensions', 'todos', 'runTests']
---
# Spring PetClinic Code Review Prompt

## Overview

Perform a comprehensive code review of this Spring Boot 3.5 PetClinic application, focusing on modern Spring Framework best practices, clean architecture, and maintainability.

## Review Areas

### 1. Spring Framework Architecture

- **Controllers**: Review Spring MVC patterns, proper use of `@Controller`, `@GetMapping`, `@PostMapping`
- **Models/Entities**: Check JPA mappings, Bean Validation annotations, inheritance from `BaseEntity`/`Person`/`NamedEntity`
- **Repositories**: Verify Spring Data JPA patterns, proper extension of `JpaRepository<Entity, ID>`
- **Dependency Injection**: Ensure constructor injection is used consistently
- **Package Structure**: Validate domain-driven package organization (`owner/`, `vet/`, `system/`)

### 2. Code Quality & Standards

- **Naming Conventions**:
  - Controllers: `[Entity]Controller` pattern
  - Tests: `[Entity]ControllerTests` pattern
  - Entities: Proper JPA entity naming
- **Documentation**: Check for comprehensive Javadoc comments
- **Error Handling**: Review exception handling and validation patterns
- **Security**: Validate input validation and CSRF protection considerations

### 3. Testing Excellence

- **Unit Tests**:
  - Use of `@WebMvcTest` for controller testing
  - MockMvc integration for HTTP testing
  - `@MockitoBean` for repository mocking
  - Hamcrest matchers usage (`hasProperty`, `hasSize`, etc.)
- **Integration Tests**:
  - `@SpringBootTest` for full application context
  - Database integration patterns
- **Test Structure**:
  - Proper `@BeforeEach` setup methods
  - Test data builders and fixtures
  - `@Nested` test classes for logical grouping
  - Both positive and negative test scenarios

### 4. Database & JPA Patterns

- **Entity Relationships**: Review `@OneToMany`, `@ManyToOne`, `@ManyToMany` mappings
- **Query Methods**: Spring Data JPA method naming conventions
- **Transaction Management**: Proper `@Transactional` usage
- **Multi-Database Support**: H2, MySQL, PostgreSQL configuration

### 5. Spring Boot Best Practices

- **Configuration**: Application properties and profiles setup
- **Auto-configuration**: Proper use of Spring Boot starters
- **Actuator**: Health checks and monitoring endpoints
- **Maven Integration**: POM.xml dependencies and build configuration

## Specific Focus Areas

### Controllers Review

For each controller class, check:

- Proper request mapping patterns
- View name consistency with Thymeleaf templates
- Model attribute handling
- Form binding and validation
- Error handling with `BindingResult`

### Model/Entity Review

For each entity class, verify:

- Proper JPA annotations (`@Entity`, `@Table`, `@Column`)
- Bean Validation constraints (`@NotBlank`, `@Pattern`, `@Size`)
- Relationship mappings and cascade settings
- Inheritance hierarchy usage
- ToString, equals, hashCode implementations

### Repository Review

For each repository interface, examine:

- Extension of appropriate Spring Data interface
- Custom query methods following naming conventions
- `@Query` annotations for complex queries
- Transaction boundaries

### Test Coverage Review

For each test class, evaluate:

- Test method naming and organization
- Mock setup and configuration
- Assertion patterns and completeness
- Edge case and error condition coverage
- Integration with Spring Test framework

## Review Criteria

### Code Smells to Identify

- Field injection instead of constructor injection
- Missing validation annotations
- Inconsistent error handling
- Poor test coverage or missing edge cases
- Tight coupling between layers
- Missing documentation
- Inconsistent naming conventions

### Best Practices to Validate

- Single Responsibility Principle adherence
- Proper separation of concerns
- Clean Architecture boundaries
- SOLID principles application
- Spring Framework idioms usage
- Test-driven development patterns

## Output Format

Please provide a structured review with:

1. **Overall Assessment**: High-level summary of code quality
2. **Strengths**: What the code does well
3. **Issues Found**: Categorized by severity (Critical, Major, Minor)
4. **Recommendations**: Specific actionable improvements
5. **Spring Boot Compliance**: Adherence to Spring Framework best practices
6. **Test Quality**: Assessment of test coverage and quality
7. **Architecture Review**: Clean architecture and design pattern usage

### Issue Categories

- **Critical**: Security vulnerabilities, major architecture violations
- **Major**: Performance issues, maintainability problems, test gaps
- **Minor**: Code style, minor optimizations, documentation improvements

## Context Information

- **Framework**: Spring Boot 3.5 with Spring MVC
- **Java Version**: Java 17+
- **Database**: H2 (default), MySQL, PostgreSQL support
- **ORM**: Spring Data JPA with Hibernate
- **Template Engine**: Thymeleaf
- **Testing**: JUnit 5, Mockito, Spring Test, MockMvc
- **Build Tool**: Maven with Maven Wrapper

Focus on maintainable, testable, and scalable Spring Boot application patterns that follow modern Java development best practices.
