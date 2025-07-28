using Microsoft.AspNetCore.Mvc;
using PetClinic.Data.Repositories;
using PetClinic.Models;

namespace PetClinic.Controllers.Api;

/// <summary>
/// API Controller for managing veterinarians
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class VetsApiController : ControllerBase
{
    private readonly IVetRepository _vets;
    private readonly ILogger<VetsApiController> _logger;

    public VetsApiController(IVetRepository vets, ILogger<VetsApiController> logger)
    {
        _vets = vets;
        _logger = logger;
    }

    /// <summary>
    /// Get all veterinarians
    /// </summary>
    /// <returns>List of veterinarians with their specialties</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Vet>>> GetVets()
    {
        try
        {
            var vets = await _vets.FindAllAsync();
            return Ok(vets);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vets");
            return StatusCode(500, "An error occurred while retrieving veterinarians");
        }
    }

    /// <summary>
    /// Get a specific veterinarian by ID
    /// </summary>
    /// <param name="id">Veterinarian ID</param>
    /// <returns>Veterinarian details with specialties</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Vet>> GetVet(int id)
    {
        try
        {
            var vet = await _vets.FindByIdAsync(id);
            if (vet == null)
            {
                return NotFound($"Veterinarian with ID {id} not found");
            }

            return Ok(vet);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vet with ID: {VetId}", id);
            return StatusCode(500, "An error occurred while retrieving the veterinarian");
        }
    }
}
