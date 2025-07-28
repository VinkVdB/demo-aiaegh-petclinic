using Microsoft.AspNetCore.Mvc;
using PetClinic.Data.Repositories;
using PetClinic.Models;

namespace PetClinic.Controllers;

public class VetsController : Controller
{
    private readonly IVetRepository _vetRepository;
    private readonly ILogger<VetsController> _logger;

    public VetsController(IVetRepository vetRepository, ILogger<VetsController> logger)
    {
        _vetRepository = vetRepository;
        _logger = logger;
    }

    /// <summary>
    /// Show vet list with pagination - HTML view endpoint
    /// </summary>
    [HttpGet("/vets.html")]
    public async Task<IActionResult> Index(int page = 1)
    {
        // Get paginated vets (matching Spring behavior)
        var paginated = await FindPaginatedAsync(page);
        return AddPaginationModel(page, paginated);
    }

    /// <summary>
    /// Show vet list as JSON - API endpoint (matches Spring @ResponseBody)
    /// </summary>
    [HttpGet("/vets")]
    public async Task<IActionResult> ShowResourcesVetList()
    {
        // Return JSON response with Vets wrapper (matches Spring behavior)
        var vets = new Vets();
        var allVets = await _vetRepository.FindAllAsync();
        vets.VetList.AddRange(allVets);
        return Json(vets);
    }

    /// <summary>
    /// Add pagination model and return vet list view
    /// </summary>
    private IActionResult AddPaginationModel(int page, IEnumerable<Vet> vets)
    {
        var vetsList = vets.ToList();
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(vetsList.Count / 5.0); // Assuming pageSize = 5
        ViewBag.TotalItems = vetsList.Count;
        ViewBag.ListVets = vetsList;
        return View("VetList", vetsList);
    }

    /// <summary>
    /// Find paginated vets (Spring equivalent)
    /// </summary>
    private async Task<IEnumerable<Vet>> FindPaginatedAsync(int page)
    {
        const int pageSize = 5;
        // For now, just get all vets as pagination is implemented simply
        var allVets = await _vetRepository.FindAllAsync();
        return allVets.Skip((page - 1) * pageSize).Take(pageSize);
    }
}
