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
    /// Test class for PetsController
    /// Migrated from Spring's PetControllerTests.java
    /// </summary>
    public class PetControllerTests
    {
        private const int TEST_OWNER_ID = 1;
        private const int TEST_PET_ID = 1;

        private readonly Mock<IOwnerRepository> _mockOwnerRepository;
        private readonly Mock<IPetTypeRepository> _mockPetTypeRepository;
        private readonly Mock<ILogger<PetsController>> _mockLogger;
        private readonly PetsController _controller;

        public PetControllerTests()
        {
            _mockOwnerRepository = new Mock<IOwnerRepository>();
            _mockPetTypeRepository = new Mock<IPetTypeRepository>();
            _mockLogger = new Mock<ILogger<PetsController>>();
            _controller = new PetsController(_mockOwnerRepository.Object, _mockPetTypeRepository.Object, _mockLogger.Object);

            // Setup mock data like Spring's @BeforeEach
            SetupMockData();
        }

        private void SetupMockData()
        {
            // Setup pet types - hamster like in Spring
            var hamster = new PetType
            {
                Id = 3,
                Name = "hamster"
            };
            _mockPetTypeRepository.Setup(r => r.FindAllAsync())
                                  .ReturnsAsync(new List<PetType> { hamster });

            // Setup owner with pets like in Spring
            var owner = new Owner();
            var pet = new Pet
            {
                Id = TEST_PET_ID,
                Name = "petty",
                Owner = owner
            };
            var dog = new Pet
            {
                Id = TEST_PET_ID + 1,
                Name = "doggy", 
                Owner = owner
            };

            owner.Pets = new List<Pet> { pet, dog };

            _mockOwnerRepository.Setup(r => r.FindByIdAsync(TEST_OWNER_ID))
                               .ReturnsAsync(owner);
        }

        [Fact]
        public async Task TestInitCreationForm()
        {
            // Act
            var result = await _controller.Create(TEST_OWNER_ID);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("CreateOrUpdatePetForm", viewResult.ViewName);
            Assert.IsType<Pet>(viewResult.Model);
        }

        [Fact]
        public async Task TestProcessCreationFormSuccess()
        {
            // Arrange
            var pet = new Pet
            {
                Name = "Betty",
                BirthDate = new DateTime(2015, 2, 12, 0, 0, 0, DateTimeKind.Utc),
                TypeId = 3  // hamster type id from setup
            };

            var owner = new Owner { Id = TEST_OWNER_ID };
            _mockOwnerRepository.Setup(r => r.FindByIdAsync(TEST_OWNER_ID))
                               .ReturnsAsync(owner);
            _mockOwnerRepository.Setup(r => r.SaveAsync(It.IsAny<Owner>()))
                               .ReturnsAsync(owner);

            // Act
            var result = await _controller.Create(TEST_OWNER_ID, pet);

            // Assert - Should redirect to owner details (Spring behavior: status().is3xxRedirection())
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal("Owners", redirectResult.ControllerName);
            Assert.Equal(TEST_OWNER_ID, redirectResult.RouteValues!["id"]);
        }

        #region ProcessCreationFormHasErrors Tests

        [Fact]
        public async Task TestProcessCreationFormWithBlankName()
        {
            // Arrange - Blank name (whitespace only)
            var pet = new Pet
            {
                Name = "\t \n",
                BirthDate = new DateTime(2015, 2, 12, 0, 0, 0, DateTimeKind.Utc)
            };

            // Simulate model validation error
            _controller.ModelState.AddModelError("Name", "required");

            // Act
            var result = await _controller.Create(TEST_OWNER_ID, pet);

            // Assert - Should return form with validation errors
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("CreateOrUpdatePetForm", viewResult.ViewName);
            Assert.False(_controller.ModelState.IsValid);
            Assert.True(_controller.ModelState.ContainsKey("Name"));
            Assert.Contains("required", _controller.ModelState["Name"]!.Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public async Task TestProcessCreationFormWithDuplicateName()
        {
            // Arrange - Duplicate name "petty" (existing pet name from setup)
            var pet = new Pet
            {
                Name = "petty",
                BirthDate = new DateTime(2015, 2, 12, 0, 0, 0, DateTimeKind.Utc)
            };

            // The duplicate validation would be handled by the controller logic
            var owner = new Owner();
            var existingPet = new Pet { Id = 1, Name = "petty" }; // Give the existing pet an ID so it's not considered "new"
            owner.Pets = new List<Pet> { existingPet };
            
            _mockOwnerRepository.Setup(r => r.FindByIdAsync(TEST_OWNER_ID))
                               .ReturnsAsync(owner);

            // Act
            var result = await _controller.Create(TEST_OWNER_ID, pet);

            // Assert - Should return form with duplicate error
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("CreateOrUpdatePetForm", viewResult.ViewName);
            Assert.False(_controller.ModelState.IsValid);
            Assert.True(_controller.ModelState.ContainsKey("Name"));
            Assert.Contains("already exists", _controller.ModelState["Name"]!.Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public async Task TestProcessCreationFormWithMissingPetType()
        {
            // Arrange - Missing pet type
            var pet = new Pet
            {
                Name = "Betty",
                BirthDate = new DateTime(2015, 2, 12, 0, 0, 0, DateTimeKind.Utc)
            };

            // Simulate required validation error for type
            _controller.ModelState.AddModelError("Type", "required");

            // Act
            var result = await _controller.Create(TEST_OWNER_ID, pet);

            // Assert - Should return form with validation errors
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("CreateOrUpdatePetForm", viewResult.ViewName);
            Assert.False(_controller.ModelState.IsValid);
            Assert.True(_controller.ModelState.ContainsKey("Type"));
            Assert.Contains("required", _controller.ModelState["Type"]!.Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public async Task TestProcessCreationFormWithInvalidBirthDate()
        {
            // Arrange - Future birth date (invalid)
            var currentDate = DateTime.Now;
            var futureBirthDate = currentDate.AddMonths(1);

            var pet = new Pet
            {
                Name = "Betty",
                BirthDate = futureBirthDate
            };

            // Simulate birth date validation error
            _controller.ModelState.AddModelError("BirthDate", "typeMismatch.birthDate");

            // Act
            var result = await _controller.Create(TEST_OWNER_ID, pet);

            // Assert - Should return form with validation errors
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("CreateOrUpdatePetForm", viewResult.ViewName);
            Assert.False(_controller.ModelState.IsValid);
            Assert.True(_controller.ModelState.ContainsKey("BirthDate"));
            Assert.Contains("typeMismatch.birthDate", _controller.ModelState["BirthDate"]!.Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public async Task TestInitUpdateForm()
        {
            // Act
            var result = await _controller.Edit(TEST_OWNER_ID, TEST_PET_ID);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("CreateOrUpdatePetForm", viewResult.ViewName);
            Assert.IsType<Pet>(viewResult.Model);
        }

        #endregion

        [Fact]
        public async Task TestProcessUpdateFormSuccess()
        {
            // Arrange
            var pet = new Pet
            {
                Id = TEST_PET_ID,
                Name = "Betty",
                BirthDate = new DateTime(2015, 2, 12, 0, 0, 0, DateTimeKind.Utc),
                TypeId = 3  // hamster type id from setup
            };

            var owner = new Owner { Id = TEST_OWNER_ID };
            var existingPet = new Pet { Id = TEST_PET_ID, Name = "petty", Owner = owner };
            owner.Pets = new List<Pet> { existingPet };

            _mockOwnerRepository.Setup(r => r.FindByIdAsync(TEST_OWNER_ID))
                               .ReturnsAsync(owner);
            _mockOwnerRepository.Setup(r => r.SaveAsync(It.IsAny<Owner>()))
                               .ReturnsAsync(owner);

            // Act
            var result = await _controller.Edit(TEST_OWNER_ID, TEST_PET_ID, pet);

            // Assert - Should redirect to owner details (Spring behavior: status().is3xxRedirection())
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal("Owners", redirectResult.ControllerName);
            Assert.Equal(TEST_OWNER_ID, redirectResult.RouteValues!["id"]);
        }

        #region ProcessUpdateFormHasErrors Tests

        [Fact]
        public async Task TestProcessUpdateFormWithInvalidBirthDate()
        {
            // Arrange - Invalid birth date format
            var pet = new Pet
            {
                Id = TEST_PET_ID,
                Name = " ",  // Also blank name
                BirthDate = DateTime.MinValue  // Invalid date
            };

            // Simulate model validation errors
            _controller.ModelState.AddModelError("BirthDate", "typeMismatch");

            // Act
            var result = await _controller.Edit(TEST_OWNER_ID, TEST_PET_ID, pet);

            // Assert - Should return form with validation errors
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("CreateOrUpdatePetForm", viewResult.ViewName);
            Assert.False(_controller.ModelState.IsValid);
            Assert.True(_controller.ModelState.ContainsKey("BirthDate"));
            Assert.Contains("typeMismatch", _controller.ModelState["BirthDate"]!.Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public async Task TestProcessUpdateFormWithBlankName()
        {
            // Arrange - Blank name
            var pet = new Pet
            {
                Id = TEST_PET_ID,
                Name = "  ",  // Blank name
                BirthDate = new DateTime(2015, 2, 12, 0, 0, 0, DateTimeKind.Utc)
            };

            // Simulate model validation error
            _controller.ModelState.AddModelError("Name", "required");

            // Act
            var result = await _controller.Edit(TEST_OWNER_ID, TEST_PET_ID, pet);

            // Assert - Should return form with validation errors
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("CreateOrUpdatePetForm", viewResult.ViewName);
            Assert.False(_controller.ModelState.IsValid);
            Assert.True(_controller.ModelState.ContainsKey("Name"));
            Assert.Contains("required", _controller.ModelState["Name"]!.Errors.Select(e => e.ErrorMessage));
        }

        #endregion
    }
}
