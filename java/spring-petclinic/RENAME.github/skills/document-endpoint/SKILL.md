---
name: document-endpoint
description: >
  Adds inline Javadoc and OpenAPI-style comments to Spring MVC controller
  methods and REST endpoints. Use this skill when asked to document, add
  comments, or annotate a controller method, REST handler, or request mapping.
---

# Spring Boot REST Endpoint Documenter

Add clear, consistent Javadoc and OpenAPI-style inline comments to Spring MVC controller methods in this PetClinic project.

## Javadoc Template for Controller Methods

Apply this structure to every `@GetMapping`, `@PostMapping`, `@PutMapping`, or `@DeleteMapping` method:

```java
/**
 * [One-sentence summary of what this endpoint does and what it returns.]
 *
 * <p>HTTP Method: GET | POST | PUT | DELETE
 * <p>Path: /owners/{ownerId}
 *
 * @param ownerId the unique identifier of the owner to retrieve
 * @param model   the Spring MVC {@link Model} used to pass data to the view
 * @return the logical Thymeleaf view name, or a redirect string
 *         (e.g. {@code "owners/ownerDetails"} or {@code "redirect:/owners/{ownerId}"})
 * @throws org.springframework.web.server.ResponseStatusException
 *         if the owner with the given ID is not found (HTTP 404)
 */
@GetMapping("/owners/{ownerId}")
public String showOwner(@PathVariable("ownerId") int ownerId, Model model) { ... }
```

## OpenAPI-Style Inline Comments

For methods that back a JSON REST API, add structured inline comments before the method:

```java
/**
 * Returns a paginated list of vets with their specialities.
 *
 * <p><b>GET /vets.json</b>
 * <ul>
 *   <li>200 OK — body: {@code {"vetList": [...]}}
 *   <li>204 No Content — when the vet list is empty
 * </ul>
 *
 * @param pageable pagination parameters (page, size, sort)
 * @return {@link ResponseEntity} wrapping a {@link Map} with key {@code "vetList"}
 */
```

## Class-Level Javadoc

Add or update the class-level Javadoc to describe the controller's responsibility:

```java
/**
 * Spring MVC controller for managing {@link Owner} entities.
 *
 * <p>Handles HTTP requests for creating, updating, searching and displaying
 * owners and their associated pets.
 *
 * @author <Your Name>
 * @see OwnerRepository
 */
@Controller
class OwnerController { ... }
```

## Rules for This Project

- **Language**: Write Javadoc in English. Do not use informal language.
- **`@param`**: Document every method parameter, including `Model`, `BindingResult`, and `@PathVariable` / `@RequestParam` parameters.
- **`@return`**: Always present. For `String` return types, describe the view name or the redirect target.
- **`@throws`**: Include only if the method explicitly throws or delegates to a handler that produces HTTP 4xx/5xx.
- **Avoid redundancy**: Do not repeat the method name verbatim as the summary. Describe *purpose*, not mechanics.
- **No `@author` on methods** — only on classes.
- **Preserve existing annotations**: Place Javadoc immediately above the Spring annotations (`@GetMapping`, etc.), not between them.
- **Line width**: Wrap Javadoc lines at 100 characters.

## Correct Annotation Order

```java
/**
 * Javadoc here.
 */
@GetMapping("/path")          // mapping annotation first
public String methodName(...) { ... }
```

## Quick Reference: Common View Name Patterns

| Scenario | Return value |
|---|---|
| Show a form | `"owners/createOrUpdateOwnerForm"` |
| Redirect after save | `"redirect:/owners/{ownerId}"` |
| List page | `"owners/ownersList"` |
| Detail page | `"owners/ownerDetails"` |
| Error / not found | `"error"` or set HTTP status via `ResponseEntity` |
