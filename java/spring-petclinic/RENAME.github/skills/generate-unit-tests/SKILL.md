---
name: generate-unit-tests
description: >
  Generates JUnit 5 unit tests for Spring Boot controllers and repositories
  following the project's established test patterns. Use this skill when asked
  to write, create, or generate tests for a controller, repository, or service
  class in this project.
---

# Spring Boot Unit Test Generator

Generate unit tests for Spring Boot components in this PetClinic project. Follow the patterns below exactly.

## Controller Tests (`@WebMvcTest`)

Use `@WebMvcTest` for controller-layer tests. Never load the full application context for controller tests.

```java
@WebMvcTest(OwnerController.class)
@DisabledInNativeImage
@DisabledInAotMode
class OwnerControllerTests {

    @Autowired
    private MockMvc mockMvc;

    @MockitoBean
    private OwnerRepository owners;   // mock every repository the controller uses

    private static final int TEST_OWNER_ID = 1;

    private Owner makeOwner() {
        Owner owner = new Owner();
        owner.setId(TEST_OWNER_ID);
        owner.setFirstName("George");
        owner.setLastName("Franklin");
        owner.setAddress("110 W. Liberty St.");
        owner.setCity("Madison");
        owner.setTelephone("6085551023");
        return owner;
    }

    @BeforeEach
    void setup() {
        given(this.owners.findById(TEST_OWNER_ID))
            .willReturn(Optional.of(makeOwner()));
    }

    @Test
    void testShowOwner() throws Exception {
        mockMvc.perform(get("/owners/{id}", TEST_OWNER_ID))
            .andExpect(status().isOk())
            .andExpect(model().attribute("owner",
                hasProperty("lastName", is("Franklin"))))
            .andExpect(view().name("owners/ownerDetails"));
    }
}
```

## Repository / Integration Tests (`@DataJpaTest`)

Use `@DataJpaTest` with `@AutoConfigureTestDatabase(replace = Replace.NONE)` for repository tests against H2.

```java
@DataJpaTest
@AutoConfigureTestDatabase(replace = Replace.NONE)
@Transactional
class OwnerRepositoryTests {

    @Autowired
    private OwnerRepository owners;

    @Test
    void findByLastNameShouldReturnOwner() {
        Page<Owner> result = owners.findByLastNameStartingWith(
            "Franklin", Pageable.ofSize(10));
        assertThat(result).isNotEmpty();
        assertThat(result.getContent().get(0).getLastName()).isEqualTo("Franklin");
    }
}
```

## Naming and Structure Rules

- Test class name: `[ComponentUnderTest]Tests` — e.g., `OwnerControllerTests`.
- Use `@BeforeEach` for shared fixture setup; keep per-test data in private helper methods.
- Follow the **Arrange / Act / Assert** (AAA) structure within every `@Test` method.
- Use BDD-style stubs: `given(mock.method(arg)).willReturn(value)` (Mockito BDDMockito).
- Use Hamcrest matchers (`hasProperty`, `hasSize`, `is`, `not`, `empty`) for model assertions.
- Use AssertJ (`assertThat`) for repository / service assertions.
- Name tests in camelCase starting with `test` or a plain verb: `testShowOwner`, `findByLastNameReturnsMatch`.

## Coverage Checklist

For each controller method, generate tests covering:

1. Happy path (valid input, expected view / redirect).
2. Validation failure (missing or invalid fields → same form view, `model().attributeHasErrors`).
3. Entity not found (mock returns `Optional.empty()` → expect HTTP 404 or redirect with error).
4. Form redirect on success (`status().is3xxRedirection()` + `view().name("redirect:...")`).

## Required Imports (controller tests)

```java
import static org.hamcrest.Matchers.*;
import static org.mockito.ArgumentMatchers.*;
import static org.mockito.BDDMockito.given;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.*;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.*;
```

## Project-Specific Notes

- The project uses `@MockitoBean` (Spring Boot 3.4+), NOT the deprecated `@MockBean`.
- Controller tests must annotate the class with `@DisabledInNativeImage` and `@DisabledInAotMode`.
- Run tests with `./mvnw test` from the project root. A single test class: `./mvnw test -Dtest=OwnerControllerTests`.
