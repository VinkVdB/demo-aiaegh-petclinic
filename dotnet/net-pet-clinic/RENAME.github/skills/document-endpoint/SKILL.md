---
name: document-endpoint
description: Generate XML documentation comments for ASP.NET Core MVC and API controller actions in the PetClinic project. Use this skill when asked to add, write, or improve documentation comments on controller methods or classes.
---

# ASP.NET Core Controller Documenter for PetClinic

Generate XML documentation comments for controller actions following the patterns established in `PetClinic/Controllers/`.

## Documentation Style

This project uses standard C# XML doc comments (`///`). All public controller classes and action methods must have XML documentation.

## MVC Controller Documentation

MVC controllers (in `PetClinic/Controllers/`) use a concise single-line `<summary>` on each action. Include the HTTP route when it adds clarity.

```csharp
/// <summary>
/// Display form to find owners
/// </summary>
[HttpGet("/owners/find")]
public IActionResult Find()

/// <summary>
/// Process find form and display results
/// Route: GET /owners?lastName=...&page=1
/// </summary>
[HttpGet("/owners")]
public async Task<IActionResult> Index(string? lastName, int page = 1)

/// <summary>
/// Display form to create new owner
/// </summary>
[HttpGet("/owners/new")]
public IActionResult Create()

/// <summary>
/// Process creation form
/// </summary>
[HttpPost("/owners/new")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create([Bind(...)] Owner owner)
```

Do NOT add `<param>` or `<returns>` tags to MVC controller actions — the pattern in this project uses summary only for MVC.

## API Controller Documentation

API controllers (in `PetClinic/Controllers/Api/`) use a fuller documentation style with `<summary>`, `<param>`, and `<returns>` tags. These appear in the Swagger UI at `/swagger`.

```csharp
/// <summary>
/// Get all owners with optional filtering by last name
/// </summary>
/// <param name="lastName">Filter by last name (optional)</param>
/// <param name="page">Page number for pagination (default: 1)</param>
/// <param name="size">Page size (default: 10)</param>
/// <returns>List of owners</returns>
[HttpGet]
public async Task<ActionResult<IEnumerable<Owner>>> GetOwners(...)

/// <summary>
/// Get a specific owner by ID
/// </summary>
/// <param name="id">Owner ID</param>
/// <returns>Owner details including pets</returns>
[HttpGet("{id}")]
public async Task<ActionResult<Owner>> GetOwner(int id)

/// <summary>
/// Create a new owner
/// </summary>
/// <param name="owner">Owner details</param>
/// <returns>Created owner</returns>
[HttpPost]
public async Task<ActionResult<Owner>> CreateOwner([FromBody] Owner owner)

/// <summary>
/// Update an existing owner
/// </summary>
/// <param name="id">Owner ID</param>
/// <param name="owner">Updated owner details</param>
/// <returns>Updated owner</returns>
[HttpPut("{id}")]
public async Task<ActionResult<Owner>> UpdateOwner(int id, [FromBody] Owner owner)

/// <summary>
/// Delete an owner
/// </summary>
/// <param name="id">Owner ID</param>
/// <returns>No content</returns>
[HttpDelete("{id}")]
public async Task<ActionResult> DeleteOwner(int id)
```

## Class-Level Documentation

```csharp
/// <summary>
/// API Controller for managing pet owners
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class OwnersApiController : ControllerBase
```

For MVC controllers, the class-level summary can be omitted or kept brief — the existing codebase does not always add one.

## Private Helper Method Documentation

Document private methods when their purpose is not immediately obvious from the name:

```csharp
/// <summary>
/// Add pagination model attributes and return owners list view
/// </summary>
private IActionResult AddPaginationModel(int page, IEnumerable<Owner> owners, int totalCount)

/// <summary>
/// Find paginated owners by last name
/// </summary>
private async Task<(IEnumerable<Owner> Owners, int TotalCount)> FindPaginatedForOwnersLastNameAsync(...)
```

## Writing Guidelines

- **Be concise**: summaries should fit on one line where possible
- **Use present tense**: "Display form" not "Displays form"
- **Start with a verb**: "Get", "Create", "Update", "Delete", "Display", "Process"
- **Mention the entity**: "Get all owners", not "Get all items"
- **For `<param>`**: state the purpose and whether optional, e.g. "Filter by last name (optional)"
- **For `<returns>`**: state what is returned, e.g. "List of owners", "Created owner", "No content"
- **Do not repeat the method name** verbatim in the summary

## Repository Interface Documentation

When documenting repository interfaces or implementations:

```csharp
/// <summary>
/// Find an owner by their unique ID, including related pets and visits
/// </summary>
/// <param name="id">The owner's primary key</param>
/// <returns>The owner, or null if not found</returns>
Task<Owner?> FindByIdAsync(int id);
```

## Model Property Documentation

For model properties, use a single-line `<summary>`:

```csharp
/// <summary>
/// Owner's street address
/// </summary>
[Required]
public string Address { get; set; } = string.Empty;
```
