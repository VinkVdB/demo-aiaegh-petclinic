using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PetClinic.Controllers;
using PetClinic.Data.Repositories;
using PetClinic.Models;
using Xunit;

namespace PetClinic.Tests.Unit
{
    /// <summary>
    /// Test class for OwnersController
    /// Migrated from Spring's OwnerControllerTests.java
    /// </summary>
    public class OwnerControllerTests
    {
        private const int TEST_OWNER_ID = 1;
        
        private readonly Mock<IOwnerRepository> _mockOwnerRepository;
        private readonly Mock<ILogger<OwnersController>> _mockLogger;
        private readonly OwnersController _controller;

        public OwnerControllerTests()
        {
            _mockOwnerRepository = new Mock<IOwnerRepository>();
            _mockLogger = new Mock<ILogger<OwnersController>>();
            _controller = new OwnersController(_mockOwnerRepository.Object, _mockLogger.Object);
        }

        private Owner CreateGeorge()
        {
            var owner = new Owner
            {
                Id = TEST_OWNER_ID,
                FirstName = "George",
                LastName = "Franklin",
                Address = "110 W. Liberty St.",
                City = "Madison",
                Telephone = "6085551023"
            };

            var petType = new PetType { Name = "dog" };
            var pet = new Pet
            {
                Id = 1,
                Name = "Max",
                BirthDate = DateTime.Now.Date,
                Type = petType,
                Owner = owner
            };

            owner.Pets = new List<Pet> { pet };

            var visit = new Visit
            {
                Date = DateTime.Now.Date,
                Pet = pet
            };
            pet.Visits = new List<Visit> { visit };

            return owner;
        }

        [Fact]
        public void TestInitCreationForm()
        {
            // Act
            var result = _controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("CreateOrUpdateOwnerForm", viewResult.ViewName);
        }

        [Fact]
        public async Task TestProcessCreationFormSuccess()
        {
            // Arrange
            var owner = new Owner
            {
                FirstName = "Joe",
                LastName = "Bloggs",
                Address = "123 Caramel Street",
                City = "London",
                Telephone = "1316761638"
            };

            var savedOwner = new Owner
            {
                Id = 1,
                FirstName = "Joe",
                LastName = "Bloggs",
                Address = "123 Caramel Street",
                City = "London",
                Telephone = "1316761638"
            };

            _mockOwnerRepository.Setup(r => r.SaveAsync(It.IsAny<Owner>()))
                               .ReturnsAsync(savedOwner);

            // Act
            var result = await _controller.Create(owner);

            // Assert - Should redirect to owner details (Spring behavior: status().is3xxRedirection())
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal(TEST_OWNER_ID, redirectResult.RouteValues!["id"]);
        }

        [Fact]
        public async Task TestProcessCreationFormHasErrors()
        {
            // Arrange - Missing required fields: address and telephone
            var owner = new Owner
            {
                FirstName = "Joe",
                LastName = "Bloggs",
                City = "London"
                // address and telephone missing
            };

            // Simulate model validation errors
            _controller.ModelState.AddModelError("Address", "The Address field is required.");
            _controller.ModelState.AddModelError("Telephone", "The Telephone field is required.");

            // Act
            var result = await _controller.Create(owner);

            // Assert - Should return form with validation errors
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("CreateOrUpdateOwnerForm", viewResult.ViewName);
            Assert.False(_controller.ModelState.IsValid);
            Assert.True(_controller.ModelState.ContainsKey("Address"));
            Assert.True(_controller.ModelState.ContainsKey("Telephone"));
        }

        [Fact]
        public void TestInitFindForm()
        {
            // Act
            var result = _controller.Find();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("FindOwners", viewResult.ViewName);
        }

        [Fact]
        public async Task TestProcessFindFormSuccess()
        {
            // Arrange
            var owners = new List<Owner> { CreateGeorge(), new Owner() };
            _mockOwnerRepository.Setup(r => r.FindByLastNameStartingWithAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                               .ReturnsAsync((owners, 2));

            // Act
            var result = await _controller.Index("", 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("OwnersList", viewResult.ViewName);
        }

        [Fact]
        public async Task TestProcessFindFormByLastName()
        {
            // Arrange - Single result should redirect to owner details
            var george = CreateGeorge();
            _mockOwnerRepository.Setup(r => r.FindByLastNameStartingWithAsync("Franklin", It.IsAny<int>(), It.IsAny<int>()))
                               .ReturnsAsync((new List<Owner> { george }, 1));

            // Act
            var result = await _controller.Index("Franklin", 1);

            // Assert - Should redirect to owner details (Spring behavior: status().is3xxRedirection())
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal(TEST_OWNER_ID, redirectResult.RouteValues!["id"]);
        }

        [Fact]
        public async Task TestProcessFindFormNoOwnersFound()
        {
            // Arrange
            _mockOwnerRepository.Setup(r => r.FindByLastNameStartingWithAsync("Unknown Surname", It.IsAny<int>(), It.IsAny<int>()))
                               .ReturnsAsync((new List<Owner>(), 0));

            // Act
            var result = await _controller.Index("Unknown Surname", 1);

            // Assert - Should return find form with error
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("FindOwners", viewResult.ViewName);
            Assert.True(_controller.ModelState.ContainsKey("LastName"));
            Assert.Contains("not found", _controller.ModelState["LastName"]!.Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public async Task TestInitUpdateOwnerForm()
        {
            // Arrange
            var george = CreateGeorge();
            _mockOwnerRepository.Setup(r => r.FindByIdAsync(TEST_OWNER_ID))
                               .ReturnsAsync(george);

            // Act
            var result = await _controller.Edit(TEST_OWNER_ID);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("CreateOrUpdateOwnerForm", viewResult.ViewName);
            var model = Assert.IsType<Owner>(viewResult.Model);
            Assert.Equal("Franklin", model.LastName);
            Assert.Equal("George", model.FirstName);
            Assert.Equal("110 W. Liberty St.", model.Address);
            Assert.Equal("Madison", model.City);
            Assert.Equal("6085551023", model.Telephone);
        }

        [Fact]
        public async Task TestProcessUpdateOwnerFormSuccess()
        {
            // Arrange
            var existingOwner = CreateGeorge();
            var updatedOwner = new Owner
            {
                Id = TEST_OWNER_ID,
                FirstName = "Joe",
                LastName = "Bloggs",
                Address = "123 Caramel Street",
                City = "London",
                Telephone = "1616291589"
            };

            _mockOwnerRepository.Setup(r => r.FindByIdAsync(TEST_OWNER_ID))
                               .ReturnsAsync(existingOwner);
            _mockOwnerRepository.Setup(r => r.SaveAsync(It.IsAny<Owner>()))
                               .ReturnsAsync(updatedOwner);

            // Act
            var result = await _controller.Edit(TEST_OWNER_ID, updatedOwner);

            // Assert - Should redirect to owner details (Spring behavior: status().is3xxRedirection())
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal(TEST_OWNER_ID, redirectResult.RouteValues!["id"]);
        }

        [Fact]
        public async Task TestProcessUpdateOwnerFormUnchangedSuccess()
        {
            // Arrange
            var existingOwner = CreateGeorge();
            _mockOwnerRepository.Setup(r => r.FindByIdAsync(TEST_OWNER_ID))
                               .ReturnsAsync(existingOwner);
            _mockOwnerRepository.Setup(r => r.SaveAsync(It.IsAny<Owner>()))
                               .ReturnsAsync(existingOwner);

            // Act - Submit without changes
            var result = await _controller.Edit(TEST_OWNER_ID, existingOwner);

            // Assert - Should still redirect (Spring behavior: status().is3xxRedirection())
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal(TEST_OWNER_ID, redirectResult.RouteValues!["id"]);
        }

        [Fact]
        public async Task TestProcessUpdateOwnerFormHasErrors()
        {
            // Arrange
            var existingOwner = CreateGeorge();
            var invalidOwner = new Owner
            {
                Id = TEST_OWNER_ID,
                FirstName = "Joe",
                LastName = "Bloggs",
                Address = "", // Empty address
                Telephone = "" // Empty telephone
            };

            _mockOwnerRepository.Setup(r => r.FindByIdAsync(TEST_OWNER_ID))
                               .ReturnsAsync(existingOwner);

            // Simulate model validation errors
            _controller.ModelState.AddModelError("Address", "The Address field is required.");
            _controller.ModelState.AddModelError("Telephone", "The Telephone field is required.");

            // Act
            var result = await _controller.Edit(TEST_OWNER_ID, invalidOwner);

            // Assert - Should return form with validation errors
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("CreateOrUpdateOwnerForm", viewResult.ViewName);
            Assert.False(_controller.ModelState.IsValid);
            Assert.True(_controller.ModelState.ContainsKey("Address"));
            Assert.True(_controller.ModelState.ContainsKey("Telephone"));
        }

        [Fact]
        public async Task TestShowOwner()
        {
            // Arrange
            var george = CreateGeorge();
            _mockOwnerRepository.Setup(r => r.FindByIdAsync(TEST_OWNER_ID))
                               .ReturnsAsync(george);

            // Act
            var result = await _controller.Details(TEST_OWNER_ID);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("OwnerDetails", viewResult.ViewName);
            var model = Assert.IsType<Owner>(viewResult.Model);
            Assert.Equal("Franklin", model.LastName);
            Assert.Equal("George", model.FirstName);
            Assert.Equal("110 W. Liberty St.", model.Address);
            Assert.Equal("Madison", model.City);
            Assert.Equal("6085551023", model.Telephone);
            Assert.NotEmpty(model.Pets);
            Assert.Contains(model.Pets, p => p.Visits.Count > 0);
        }

        [Fact]
        public async Task TestProcessUpdateOwnerFormWithIdMismatch()
        {
            // Arrange
            const int pathOwnerId = 1;
            var owner = new Owner
            {
                Id = 2, // Different ID than path
                FirstName = "John",
                LastName = "Doe",
                Address = "Center Street",
                City = "New York",
                Telephone = "0123456789"
            };

            var existingOwner = new Owner { Id = pathOwnerId };
            _mockOwnerRepository.Setup(r => r.FindByIdAsync(pathOwnerId))
                               .ReturnsAsync(existingOwner);

            // Act
            var result = await _controller.Edit(pathOwnerId, owner);

            // Assert - Should redirect back to edit form with error (Spring behavior)
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Edit", redirectResult.ActionName);
            Assert.Equal(pathOwnerId, redirectResult.RouteValues!["id"]);
            // In a real scenario, TempData would contain an error message
        }
    }
}
