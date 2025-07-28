using Microsoft.AspNetCore.Mvc;
using PetClinic.Data.Repositories;
using PetClinic.Models;

namespace PetClinic.Controllers.Api;

/// <summary>
/// API Controller for managing pet types
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PetTypesApiController : ControllerBase
{
    private readonly IPetTypeRepository _petTypes;
    private readonly ILogger<PetTypesApiController> _logger;

    public PetTypesApiController(IPetTypeRepository petTypes, ILogger<PetTypesApiController> logger)
    {
        _petTypes = petTypes;
        _logger = logger;
    }

    /// <summary>
    /// Get all pet types
    /// </summary>
    /// <returns>List of available pet types</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PetType>>> GetPetTypes()
    {
        try
        {
            var petTypes = await _petTypes.FindAllAsync();
            return Ok(petTypes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pet types");
            return StatusCode(500, "An error occurred while retrieving pet types");
        }
    }

    /// <summary>
    /// Get a specific pet type by ID
    /// </summary>
    /// <param name="id">Pet type ID</param>
    /// <returns>Pet type details</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<PetType>> GetPetType(int id)
    {
        try
        {
            var petType = await _petTypes.FindByIdAsync(id);
            if (petType == null)
            {
                return NotFound($"Pet type with ID {id} not found");
            }

            return Ok(petType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pet type with ID: {PetTypeId}", id);
            return StatusCode(500, "An error occurred while retrieving the pet type");
        }
    }
}
