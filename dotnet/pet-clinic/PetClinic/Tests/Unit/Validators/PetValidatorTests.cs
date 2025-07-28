using System.ComponentModel.DataAnnotations;
using Xunit;
using PetClinic.Models;
using PetClinic.Validators;

namespace PetClinic.Tests.Unit.Validators;

/// <summary>
/// Test class for PetValidator
/// 
/// Migrated from Spring PetClinic PetValidatorTests.java
/// </summary>
public class PetValidatorTests
{
    private readonly Pet _pet;
    private readonly PetType _petType;
    
    private const string PetName = "Buddy";
    private const string PetTypeName = "Dog";
    private static readonly DateTime PetBirthDate = new(1990, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

    public PetValidatorTests()
    {
        _pet = new Pet();
        _petType = new PetType();
    }

    [Fact]
    public void TestValidate()
    {
        // Arrange
        _petType.Name = PetTypeName;
        _pet.Name = PetName;
        _pet.Type = _petType;
        _pet.BirthDate = PetBirthDate;

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = PetValidator.Validate(_pet, validationResults);

        // Assert
        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

    [Fact]
    public void TestValidateWithInvalidPetName()
    {
        // Arrange
        _petType.Name = PetTypeName;
        _pet.Name = "";
        _pet.Type = _petType;
        _pet.BirthDate = PetBirthDate;

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = PetValidator.Validate(_pet, validationResults);

        // Assert
        Assert.False(isValid);
        Assert.Single(validationResults);
        
        var violation = validationResults[0];
        Assert.Contains("Name", violation.MemberNames);
        Assert.Equal("required", violation.ErrorMessage);
    }

    [Fact]
    public void TestValidateWithInvalidPetType()
    {
        // Arrange
        _pet.Name = PetName;
        _pet.Type = null;
        _pet.BirthDate = PetBirthDate;

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = PetValidator.Validate(_pet, validationResults);

        // Assert
        Assert.False(isValid);
        Assert.Single(validationResults);
        
        var violation = validationResults[0];
        Assert.Contains("Type", violation.MemberNames);
        Assert.Equal("required", violation.ErrorMessage);
    }

    [Fact]
    public void TestValidateWithInvalidBirthDate()
    {
        // Arrange
        _petType.Name = PetTypeName;
        _pet.Name = PetName;
        _pet.Type = _petType;
        _pet.BirthDate = null;

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = PetValidator.Validate(_pet, validationResults);

        // Assert
        Assert.False(isValid);
        Assert.Single(validationResults);
        
        var violation = validationResults[0];
        Assert.Contains("BirthDate", violation.MemberNames);
        Assert.Equal("required", violation.ErrorMessage);
    }

    [Fact]
    public void TestValidateWithWhitespacePetName()
    {
        // Arrange
        _petType.Name = PetTypeName;
        _pet.Name = "   "; // Only whitespace
        _pet.Type = _petType;
        _pet.BirthDate = PetBirthDate;

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = PetValidator.Validate(_pet, validationResults);

        // Assert
        Assert.False(isValid);
        Assert.Single(validationResults);
        
        var violation = validationResults[0];
        Assert.Contains("Name", violation.MemberNames);
        Assert.Equal("required", violation.ErrorMessage);
    }

    [Fact]
    public void TestSupportsMethod()
    {
        // Act & Assert
        Assert.True(PetValidator.Supports(typeof(Pet)));
        Assert.False(PetValidator.Supports(typeof(Owner)));
        Assert.False(PetValidator.Supports(typeof(string)));
    }

    [Fact]
    public void TestValidateOverloadMethod()
    {
        // Arrange
        _petType.Name = PetTypeName;
        _pet.Name = PetName;
        _pet.Type = _petType;
        _pet.BirthDate = PetBirthDate;

        // Act
        var validationResults = PetValidator.Validate(_pet);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void TestValidateOverloadMethodWithErrors()
    {
        // Arrange
        _pet.Name = "";
        _pet.Type = null;
        _pet.BirthDate = null;

        // Act
        var validationResults = PetValidator.Validate(_pet);

        // Assert
        Assert.Equal(3, validationResults.Count);
        
        var memberNames = validationResults.SelectMany(vr => vr.MemberNames).ToList();
        Assert.Contains("Name", memberNames);
        Assert.Contains("Type", memberNames);
        Assert.Contains("BirthDate", memberNames);
    }
}
