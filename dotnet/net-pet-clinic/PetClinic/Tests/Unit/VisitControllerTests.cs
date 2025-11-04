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
    /// Test class for VisitsController
    /// 
    /// Migrated from: org.springframework.samples.petclinic.owner.VisitControllerTests
    /// </summary>
    public class VisitControllerTests
    {
        private const int TEST_OWNER_ID = 1;
        private const int TEST_PET_ID = 1;
        
        private readonly Mock<IOwnerRepository> _mockOwnerRepository;
        private readonly Mock<ILogger<VisitsController>> _mockLogger;
        private readonly VisitsController _controller;

        public VisitControllerTests()
        {
            _mockOwnerRepository = new Mock<IOwnerRepository>();
            _mockLogger = new Mock<ILogger<VisitsController>>();
            _controller = new VisitsController(_mockOwnerRepository.Object, _mockLogger.Object);
        }

        /// <summary>
        /// Spring: testInitNewVisitForm
        /// Tests GET /owners/{ownerId}/pets/{petId}/visits/new
        /// </summary>
        [Fact]
        public async Task TestInitNewVisitForm()
        {
            // Arrange - Set up owner with pet (matches Spring @BeforeEach setup)
            var owner = new Owner();
            var pet = new Pet();
            owner.AddPet(pet);
            pet.Id = TEST_PET_ID; // Set ID after adding to collection (Spring pattern)
            
            _mockOwnerRepository.Setup(r => r.FindByIdAsync(TEST_OWNER_ID))
                               .ReturnsAsync(owner);

            // Act
            var result = await _controller.Create(TEST_OWNER_ID, TEST_PET_ID);

            // Assert - Should return CreateOrUpdateVisitForm view
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("CreateOrUpdateVisitForm", viewResult.ViewName);
            
            // Verify the visit model is provided
            Assert.IsType<Visit>(viewResult.Model);
            
            // Verify ViewBag data matches Spring @ModelAttribute behavior
            Assert.Equal(pet, viewResult.ViewData["Pet"]);
            Assert.Equal(owner, viewResult.ViewData["Owner"]);
        }

        /// <summary>
        /// Spring: testProcessNewVisitFormSuccess
        /// Tests POST /owners/{ownerId}/pets/{petId}/visits/new with valid data
        /// Spring test uses: .param("name", "George").param("description", "Visit Description")
        /// Note: Spring uses "name" param but Visit entity has "description", this is the Spring test pattern
        /// </summary>
        [Fact]
        public async Task TestProcessNewVisitFormSuccess()
        {
            // Arrange - Set up owner with pet
            var owner = new Owner();
            var pet = new Pet();
            owner.AddPet(pet);
            pet.Id = TEST_PET_ID; // Set ID after adding to collection
            
            _mockOwnerRepository.Setup(r => r.FindByIdAsync(TEST_OWNER_ID))
                               .ReturnsAsync(owner);

            // Arrange - Create valid visit (matches Spring test params)
            var visit = new Visit
            {
                Date = DateTime.Now.Date,
                Description = "Visit Description"
            };

            // Act
            var result = await _controller.Create(TEST_OWNER_ID, TEST_PET_ID, visit);

            // Assert - Should redirect to owner details (Spring: "redirect:/owners/{ownerId}")
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal("Owners", redirectResult.ControllerName);
            Assert.Equal(TEST_OWNER_ID, redirectResult.RouteValues!["id"]);
            
            // Verify repository save was called
            _mockOwnerRepository.Verify(r => r.SaveAsync(owner), Times.Once);
        }

        /// <summary>
        /// Spring: testProcessNewVisitFormHasErrors
        /// Tests POST /owners/{ownerId}/pets/{petId}/visits/new with invalid data
        /// Spring test only provides "name" param (missing required "description")
        /// </summary>
        [Fact]
        public async Task TestProcessNewVisitFormHasErrors()
        {
            // Arrange - Set up owner with pet
            var owner = new Owner();
            var pet = new Pet();
            owner.AddPet(pet);
            pet.Id = TEST_PET_ID; // Set ID after adding to collection
            
            _mockOwnerRepository.Setup(r => r.FindByIdAsync(TEST_OWNER_ID))
                               .ReturnsAsync(owner);

            // Arrange - Create invalid visit (missing description, matches Spring test)
            var visit = new Visit
            {
                Date = DateTime.Now.Date
                // Description is missing, should cause validation error
            };

            // Manually add model state error to simulate validation failure
            _controller.ModelState.AddModelError("Description", "The Description field is required.");

            // Act
            var result = await _controller.Create(TEST_OWNER_ID, TEST_PET_ID, visit);

            // Assert - Should return form with errors (Spring: model().attributeHasErrors("visit"))
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("CreateOrUpdateVisitForm", viewResult.ViewName);
            Assert.False(_controller.ModelState.IsValid);
            Assert.True(_controller.ModelState.ContainsKey("Description"));
            
            // Verify ViewBag data is set for error case
            Assert.Equal(pet, viewResult.ViewData["Pet"]);
            Assert.Equal(owner, viewResult.ViewData["Owner"]);
            
            // Verify repository save was NOT called on error
            _mockOwnerRepository.Verify(r => r.SaveAsync(It.IsAny<Owner>()), Times.Never);
        }
    }
}
