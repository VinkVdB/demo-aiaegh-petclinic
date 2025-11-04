using Microsoft.AspNetCore.Mvc;
using PetClinic.Data.Repositories;
using PetClinic.Models;

namespace PetClinic.Controllers.Api;

/// <summary>
/// API Controller for managing pet owners
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class OwnersApiController : ControllerBase
{
    private readonly IOwnerRepository _owners;
    private readonly ILogger<OwnersApiController> _logger;

    public OwnersApiController(IOwnerRepository owners, ILogger<OwnersApiController> logger)
    {
        _owners = owners;
        _logger = logger;
    }

    /// <summary>
    /// Get all owners with optional filtering by last name
    /// </summary>
    /// <param name="lastName">Filter by last name (optional)</param>
    /// <param name="page">Page number for pagination (default: 1)</param>
    /// <param name="size">Page size (default: 10)</param>
    /// <returns>List of owners</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Owner>>> GetOwners(
        [FromQuery] string? lastName = null, 
        [FromQuery] int page = 1,
        [FromQuery] int size = 10)
    {
        try
        {
            if (string.IsNullOrEmpty(lastName))
            {
                // If no lastName filter, get all owners
                var allOwners = await _owners.FindAllAsync();
                return Ok(allOwners);
            }

            // Convert to 0-based page index for repository
            var result = await _owners.FindByLastNameStartingWithAsync(lastName, page - 1, size);
            return Ok(result.Owners);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving owners with lastName: {LastName}", lastName);
            return StatusCode(500, "An error occurred while retrieving owners");
        }
    }

    /// <summary>
    /// Get a specific owner by ID
    /// </summary>
    /// <param name="id">Owner ID</param>
    /// <returns>Owner details including pets</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Owner>> GetOwner(int id)
    {
        try
        {
            var owner = await _owners.FindByIdAsync(id);
            if (owner == null)
            {
                return NotFound($"Owner with ID {id} not found");
            }

            return Ok(owner);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving owner with ID: {OwnerId}", id);
            return StatusCode(500, "An error occurred while retrieving the owner");
        }
    }

    /// <summary>
    /// Create a new owner
    /// </summary>
    /// <param name="owner">Owner details</param>
    /// <returns>Created owner</returns>
    [HttpPost]
    public async Task<ActionResult<Owner>> CreateOwner([FromBody] Owner owner)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _owners.SaveAsync(owner);
            return CreatedAtAction(nameof(GetOwner), new { id = owner.Id }, owner);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating owner: {@Owner}", owner);
            return StatusCode(500, "An error occurred while creating the owner");
        }
    }

    /// <summary>
    /// Update an existing owner
    /// </summary>
    /// <param name="id">Owner ID</param>
    /// <param name="owner">Updated owner details</param>
    /// <returns>Updated owner</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<Owner>> UpdateOwner(int id, [FromBody] Owner owner)
    {
        try
        {
            if (id != owner.Id)
            {
                return BadRequest("ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingOwner = await _owners.FindByIdAsync(id);
            if (existingOwner == null)
            {
                return NotFound($"Owner with ID {id} not found");
            }

            await _owners.SaveAsync(owner);
            return Ok(owner);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating owner with ID: {OwnerId}", id);
            return StatusCode(500, "An error occurred while updating the owner");
        }
    }

    /// <summary>
    /// Delete an owner
    /// </summary>
    /// <param name="id">Owner ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteOwner(int id)
    {
        try
        {
            var owner = await _owners.FindByIdAsync(id);
            if (owner == null)
            {
                return NotFound($"Owner with ID {id} not found");
            }

            await _owners.DeleteAsync(owner);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting owner with ID: {OwnerId}", id);
            return StatusCode(500, "An error occurred while deleting the owner");
        }
    }
}
