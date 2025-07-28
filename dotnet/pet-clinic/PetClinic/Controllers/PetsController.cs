using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PetClinic.Data.Repositories;
using PetClinic.Models;

namespace PetClinic.Controllers;

public class PetsController : Controller
{
    private const string ViewsPetsCreateOrUpdateForm = "CreateOrUpdatePetForm";
    private readonly IOwnerRepository _owners;
    private readonly IPetTypeRepository _types;
    private readonly ILogger<PetsController> _logger;

    public PetsController(IOwnerRepository owners, IPetTypeRepository types, ILogger<PetsController> logger)
    {
        _owners = owners;
        _types = types;
        _logger = logger;
    }

    /// <summary>
    /// Display form to create new pet for owner
    /// </summary>
    [HttpGet("/owners/{ownerId:int}/pets/new")]
    public async Task<IActionResult> Create(int ownerId)
    {
        var owner = await _owners.FindByIdAsync(ownerId);
        if (owner == null)
        {
            throw new ArgumentException($"Owner not found with id: {ownerId}. Please ensure the ID is correct");
        }

        var pet = new Pet();
        var petTypes = await _types.FindAllAsync();
        
        ViewBag.Owner = owner;
        ViewBag.Types = petTypes.Select(t => new SelectListItem 
        { 
            Value = t.Id.ToString(), 
            Text = t.Name 
        }).ToList();
        return View(ViewsPetsCreateOrUpdateForm, pet);
    }

    /// <summary>
    /// Process creation form for new pet
    /// </summary>
    [HttpPost("/owners/{ownerId:int}/pets/new")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int ownerId, [Bind("Name,BirthDate,TypeId")] Pet pet)
    {
        var owner = await _owners.FindByIdAsync(ownerId);
        if (owner == null)
        {
            throw new ArgumentException($"Owner not found with id: {ownerId}. Please ensure the ID is correct");
        }

        // Validation: Check for duplicate pet name
        if (!string.IsNullOrWhiteSpace(pet.Name) && pet.IsNew && owner.GetPet(pet.Name, true) != null)
        {
            ModelState.AddModelError("Name", "already exists");
        }

        // Validation: Birth date cannot be in the future
        if (pet.BirthDate.HasValue && pet.BirthDate.Value > DateTime.Today)
        {
            ModelState.AddModelError("BirthDate", "Birth date cannot be in the future");
        }

        if (!ModelState.IsValid)
        {
            var petTypes = await _types.FindAllAsync();
            ViewBag.Owner = owner;
            ViewBag.Types = petTypes.Select(t => new SelectListItem 
            { 
                Value = t.Id.ToString(), 
                Text = t.Name 
            }).ToList();
            return View(ViewsPetsCreateOrUpdateForm, pet);
        }

        owner.AddPet(pet);
        await _owners.SaveAsync(owner);
        return RedirectToAction("Details", "Owners", new { id = ownerId });
    }

    /// <summary>
    /// Display form to edit existing pet
    /// </summary>
    [HttpGet("/owners/{ownerId:int}/pets/{petId:int}/edit")]
    public async Task<IActionResult> Edit(int ownerId, int petId)
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

        var petTypes = await _types.FindAllAsync();
        ViewBag.Owner = owner;
        ViewBag.Types = petTypes.Select(t => new SelectListItem 
        { 
            Value = t.Id.ToString(), 
            Text = t.Name 
        }).ToList();
        return View(ViewsPetsCreateOrUpdateForm, pet);
    }

    /// <summary>
    /// Process edit form for existing pet
    /// </summary>
    [HttpPost("/owners/{ownerId:int}/pets/{petId:int}/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int ownerId, int petId, [Bind("Id,Name,BirthDate,TypeId")] Pet pet)
    {
        var owner = await _owners.FindByIdAsync(ownerId);
        if (owner == null)
        {
            throw new ArgumentException($"Owner not found with id: {ownerId}. Please ensure the ID is correct");
        }

        // Validation: Check for duplicate pet name (excluding current pet)
        if (!string.IsNullOrWhiteSpace(pet.Name))
        {
            var existingPet = owner.GetPet(pet.Name, false);
            if (existingPet != null && existingPet.Id != pet.Id)
            {
                ModelState.AddModelError("Name", "already exists");
            }
        }

        // Validation: Birth date cannot be in the future
        if (pet.BirthDate.HasValue && pet.BirthDate.Value > DateTime.Today)
        {
            ModelState.AddModelError("BirthDate", "Birth date cannot be in the future");
        }

        if (!ModelState.IsValid)
        {
            var petTypes = await _types.FindAllAsync();
            ViewBag.Owner = owner;
            ViewBag.Types = petTypes.Select(t => new SelectListItem 
            { 
                Value = t.Id.ToString(), 
                Text = t.Name 
            }).ToList();
            return View(ViewsPetsCreateOrUpdateForm, pet);
        }

        UpdatePetDetails(owner, pet);
        await _owners.SaveAsync(owner);
        return RedirectToAction("Details", "Owners", new { id = ownerId });
    }

    /// <summary>
    /// Updates the pet details if it exists or adds a new pet to the owner.
    /// </summary>
    private void UpdatePetDetails(Owner owner, Pet pet)
    {
        if (pet.Id.HasValue)
        {
            var existingPet = owner.GetPet(pet.Id.Value);
            if (existingPet != null)
            {
                // Update existing pet's properties
                existingPet.Name = pet.Name;
                existingPet.BirthDate = pet.BirthDate;
                existingPet.TypeId = pet.TypeId;
            }
            else
            {
                owner.AddPet(pet);
            }
        }
        else
        {
            owner.AddPet(pet);
        }
    }
}
