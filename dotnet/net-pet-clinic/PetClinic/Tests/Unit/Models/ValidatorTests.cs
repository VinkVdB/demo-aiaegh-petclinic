using System.ComponentModel.DataAnnotations;
using Xunit;
using PetClinic.Models;

namespace PetClinic.Tests.Unit.Models;

/// <summary>
/// Simple test to make sure that Data Annotations validation is working
/// (useful when upgrading to a new version of .NET Data Annotations)
/// 
/// Migrated from Spring PetClinic ValidatorTests.java
/// </summary>
public class ValidatorTests
{
    [Fact]
    public void ShouldNotValidateWhenFirstNameEmpty()
    {
        // Arrange
        var owner = new Owner
        {
            FirstName = "",
            LastName = "smith",
            Address = "123 Main St",
            City = "Springfield",
            Telephone = "1234567890"
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(owner);
        var isValid = Validator.TryValidateObject(owner, validationContext, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Single(validationResults);
        
        var violation = validationResults[0];
        Assert.Contains("FirstName", violation.MemberNames);
        Assert.Equal("The FirstName field is required.", violation.ErrorMessage);
    }

    [Fact]
    public void ShouldNotValidateWhenLastNameEmpty()
    {
        // Arrange
        var owner = new Owner
        {
            FirstName = "John",
            LastName = "",
            Address = "123 Main St",
            City = "Springfield",
            Telephone = "1234567890"
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(owner);
        var isValid = Validator.TryValidateObject(owner, validationContext, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Single(validationResults);
        
        var violation = validationResults[0];
        Assert.Contains("LastName", violation.MemberNames);
        Assert.Equal("The LastName field is required.", violation.ErrorMessage);
    }

    [Fact]
    public void ShouldValidateWhenAllRequiredFieldsProvided()
    {
        // Arrange
        var owner = new Owner
        {
            FirstName = "John",
            LastName = "Smith",
            Address = "123 Main St",
            City = "Springfield",
            Telephone = "1234567890"
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(owner);
        var isValid = Validator.TryValidateObject(owner, validationContext, validationResults, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

    [Fact]
    public void ShouldNotValidateWhenAddressEmpty()
    {
        // Arrange
        var owner = new Owner
        {
            FirstName = "John",
            LastName = "Smith",
            Address = "",
            City = "Springfield",
            Telephone = "1234567890"
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(owner);
        var isValid = Validator.TryValidateObject(owner, validationContext, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Single(validationResults);
        
        var violation = validationResults[0];
        Assert.Contains("Address", violation.MemberNames);
        Assert.Equal("The Address field is required.", violation.ErrorMessage);
    }

    [Fact]
    public void ShouldNotValidateWhenTelephoneInvalid()
    {
        // Arrange
        var owner = new Owner
        {
            FirstName = "John",
            LastName = "Smith",
            Address = "123 Main St",
            City = "Springfield",
            Telephone = "123" // Invalid - too short
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(owner);
        var isValid = Validator.TryValidateObject(owner, validationContext, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Single(validationResults);
        
        var violation = validationResults[0];
        Assert.Contains("Telephone", violation.MemberNames);
        Assert.Equal("Telephone must be exactly 10 digits.", violation.ErrorMessage);
    }
}
