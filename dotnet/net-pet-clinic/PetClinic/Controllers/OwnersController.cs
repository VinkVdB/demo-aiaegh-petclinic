using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PetClinic.Data.Repositories;
using PetClinic.Models;
using System.ComponentModel.DataAnnotations;

namespace PetClinic.Controllers;

public class OwnersController : Controller
{
    private const string ViewsOwnerCreateOrUpdateForm = "CreateOrUpdateOwnerForm";
    private readonly IOwnerRepository _owners;
    private readonly ILogger<OwnersController> _logger;

    public OwnersController(IOwnerRepository owners, ILogger<OwnersController> logger)
    {
        _owners = owners;
        _logger = logger;
    }

    /// <summary>
    /// Display form to find owners
    /// </summary>
    [HttpGet("/owners/find")]
    public IActionResult Find()
    {
        return View("FindOwners");
    }

    /// <summary>
    /// Process find form and display results
    /// Route: GET /owners?lastName=...&page=1
    /// </summary>
    [HttpGet("/owners")]
    public async Task<IActionResult> Index(string? lastName, int page = 1)
    {
        // Allow parameterless GET request for /owners to return all records
        if (string.IsNullOrEmpty(lastName))
        {
            lastName = ""; // empty string signifies broadest possible search
        }

        // Find owners by last name with pagination
        var ownersResults = await FindPaginatedForOwnersLastNameAsync(page, lastName);
        
        if (!ownersResults.Owners.Any())
        {
            // No owners found
            ModelState.AddModelError("LastName", "not found");
            return View("FindOwners");
        }

        if (ownersResults.Owners.Count() == 1)
        {
            // 1 owner found - redirect to details
            var owner = ownersResults.Owners.First();
            return RedirectToAction("Details", new { id = owner.Id });
        }

        // Multiple owners found - show paginated list
        return AddPaginationModel(page, ownersResults.Owners, ownersResults.TotalCount);
    }

    /// <summary>
    /// Display form to create new owner
    /// </summary>
    [HttpGet("/owners/new")]
    public IActionResult Create()
    {
        return View(ViewsOwnerCreateOrUpdateForm);
    }

    /// <summary>
    /// Process creation form
    /// </summary>
    [HttpPost("/owners/new")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("FirstName,LastName,Address,City,Telephone")] Owner owner)
    {
        if (!ModelState.IsValid)
        {
            return View(ViewsOwnerCreateOrUpdateForm, owner);
        }

        var savedOwner = await _owners.SaveAsync(owner);
        return RedirectToAction("Details", new { id = savedOwner.Id });
    }

    /// <summary>
    /// Display owner details with pets
    /// </summary>
    [HttpGet("/owners/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var owner = await _owners.FindByIdAsync(id);
        if (owner == null)
        {
            return NotFound();
        }
        
        return View("OwnerDetails", owner);
    }

    /// <summary>
    /// Display form to edit owner
    /// </summary>
    [HttpGet("/owners/{id:int}/edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var owner = await _owners.FindByIdAsync(id);
        if (owner == null)
        {
            return NotFound();
        }
        
        return View(ViewsOwnerCreateOrUpdateForm, owner);
    }

    /// <summary>
    /// Process edit form
    /// </summary>
    [HttpPost("/owners/{id:int}/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Address,City,Telephone")] Owner owner)
    {
        if (!ModelState.IsValid)
        {
            return View(ViewsOwnerCreateOrUpdateForm, owner);
        }

        if (owner.Id != id)
        {
            ModelState.AddModelError("Id", "The owner ID in the form does not match the URL.");
            return RedirectToAction("Edit", new { id });
        }

        owner.Id = id;
        await _owners.SaveAsync(owner);
        return RedirectToAction("Details", new { id });
    }

    /// <summary>
    /// Add pagination model attributes and return owners list view
    /// </summary>
    private IActionResult AddPaginationModel(int page, IEnumerable<Owner> owners, int totalCount)
    {
        var ownersList = owners.ToList();
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalCount / 5.0); // pageSize = 5
        ViewBag.TotalItems = totalCount;
        ViewBag.ListOwners = ownersList;
        return View("OwnersList", ownersList);
    }

    /// <summary>
    /// Find paginated owners by last name (Spring equivalent)
    /// </summary>
    private async Task<(IEnumerable<Owner> Owners, int TotalCount)> FindPaginatedForOwnersLastNameAsync(int page, string lastName)
    {
        const int pageSize = 5;
        return await _owners.FindByLastNameStartingWithAsync(lastName, page, pageSize);
    }
}
