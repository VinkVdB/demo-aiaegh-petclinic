using PetClinic.Models;
using System.ComponentModel.DataAnnotations;

namespace PetClinic.Validators;

/// <summary>
/// Custom validator for Pet forms.
/// We're not using Data Annotations here because it is easier to define such
/// validation rules in C#, following the Spring PetClinic pattern.
/// 
/// Migrated from Spring PetClinic PetValidator.java
/// </summary>
public static class PetValidator
{
    private const string Required = "required";

    /// <summary>
    /// Validates the Pet instance and populates validation results
    /// </summary>
    /// <param name="pet">The pet to validate</param>
    /// <param name="validationResults">Collection to store validation errors</param>
    /// <returns>True if validation passes, false otherwise</returns>
    public static bool Validate(Pet pet, ICollection<ValidationResult> validationResults)
    {
        if (pet == null)
        {
            throw new ArgumentNullException(nameof(pet));
        }

        bool isValid = true;

        // Name validation
        if (string.IsNullOrWhiteSpace(pet.Name))
        {
            validationResults.Add(new ValidationResult(Required, new[] { "Name" }));
            isValid = false;
        }

        // Type validation - only required for new pets
        if (pet.IsNew && pet.Type == null)
        {
            validationResults.Add(new ValidationResult(Required, new[] { "Type" }));
            isValid = false;
        }

        // Birth date validation
        if (pet.BirthDate == null)
        {
            validationResults.Add(new ValidationResult(Required, new[] { "BirthDate" }));
            isValid = false;
        }

        return isValid;
    }

    /// <summary>
    /// Validates the Pet instance and returns validation results
    /// </summary>
    /// <param name="pet">The pet to validate</param>
    /// <returns>Collection of validation results</returns>
    public static ICollection<ValidationResult> Validate(Pet pet)
    {
        var validationResults = new List<ValidationResult>();
        Validate(pet, validationResults);
        return validationResults;
    }

    /// <summary>
    /// Checks if this validator supports the given type
    /// </summary>
    /// <param name="type">The type to check</param>
    /// <returns>True if the type is assignable from Pet</returns>
    public static bool Supports(Type type)
    {
        return typeof(Pet).IsAssignableFrom(type);
    }
}
