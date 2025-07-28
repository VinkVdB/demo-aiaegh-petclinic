using Microsoft.AspNetCore.Mvc;
using PetClinic.Data.Repositories;
using PetClinic.Models;

namespace PetClinic.Controllers;

public class VisitsController : Controller
{
    private readonly IOwnerRepository _owners;
    private readonly ILogger<VisitsController> _logger;

    public VisitsController(IOwnerRepository owners, ILogger<VisitsController> logger)
    {
        _owners = owners;
        _logger = logger;
    }

    /// <summary>
    /// Display form to create new visit for pet
    /// </summary>
    [HttpGet("/owners/{ownerId:int}/pets/{petId:int}/visits/new")]
    public async Task<IActionResult> Create(int ownerId, int petId)
    {
        var owner = await _owners.FindByIdAsync(ownerId);
        if (owner == null)
        {
            throw new ArgumentException($"Owner not found with id: {ownerId}. Please ensure the ID is correct");
        }

        var pet = owner.GetPet(petId);
        if (pet == null)
        {
            throw new ArgumentException($"Pet not found with id: {petId} for owner {ownerId}");
        }

        var visit = new Visit();
        ViewBag.Pet = pet;
        ViewBag.Owner = owner;
        return View("CreateOrUpdateVisitForm", visit);
    }

    /// <summary>
    /// Process creation form for new visit
    /// </summary>
    [HttpPost("/owners/{ownerId:int}/pets/{petId:int}/visits/new")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int ownerId, int petId, [Bind("Date,Description")] Visit visit)
    {
        var owner = await _owners.FindByIdAsync(ownerId);
        if (owner == null)
        {
            throw new ArgumentException($"Owner not found with id: {ownerId}. Please ensure the ID is correct");
        }

        var pet = owner.GetPet(petId);
        if (pet == null)
        {
            throw new ArgumentException($"Pet not found with id: {petId} for owner {ownerId}");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Pet = pet;
            ViewBag.Owner = owner;
            return View("CreateOrUpdateVisitForm", visit);
        }

        owner.AddVisit(petId, visit);
        await _owners.SaveAsync(owner);
        return RedirectToAction("Details", "Owners", new { id = ownerId });
    }
}
