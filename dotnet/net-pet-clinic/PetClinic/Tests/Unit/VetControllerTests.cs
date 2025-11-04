using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PetClinic.Controllers;
using PetClinic.Data.Repositories;
using PetClinic.Models;
using System.Text.Json;
using Xunit;

namespace PetClinic.Tests.Unit
{
    /// <summary>
    /// Test class for VetsController
    /// 
    /// Migrated from: org.springframework.samples.petclinic.vet.VetControllerTests
    /// </summary>
    public class VetControllerTests
    {
        private readonly Mock<IVetRepository> _mockVetRepository;
        private readonly Mock<ILogger<VetsController>> _mockLogger;
        private readonly VetsController _controller;

        public VetControllerTests()
        {
            _mockVetRepository = new Mock<IVetRepository>();
            _mockLogger = new Mock<ILogger<VetsController>>();
            _controller = new VetsController(_mockVetRepository.Object, _mockLogger.Object);
        }

        /// <summary>
        /// Create test vet "James Carter" (matches Spring james() method)
        /// </summary>
        private Vet CreateJames()
        {
            return new Vet
            {
                Id = 1,
                FirstName = "James",
                LastName = "Carter"
            };
        }

        /// <summary>
        /// Create test vet "Helen Leary" with radiology specialty (matches Spring helen() method)
        /// </summary>
        private Vet CreateHelen()
        {
            var helen = new Vet
            {
                Id = 2,
                FirstName = "Helen",
                LastName = "Leary"
            };
            
            var radiology = new Specialty
            {
                Id = 1,
                Name = "radiology"
            };
            
            helen.AddSpecialty(radiology);
            return helen;
        }

        /// <summary>
        /// Spring: testShowVetListHtml
        /// Tests GET /vets.html?page=1 - should return HTML view with vet list
        /// </summary>
        [Fact]
        public async Task TestShowVetListHtml()
        {
            // Arrange - Set up mock repository (matches Spring @BeforeEach setup)
            var vets = new List<Vet> { CreateJames(), CreateHelen() };
            _mockVetRepository.Setup(r => r.FindAllAsync())
                             .ReturnsAsync(vets);

            // Act - Call the HTML endpoint (Spring: MockMvcRequestBuilders.get("/vets.html?page=1"))
            var result = await _controller.Index(page: 1);

            // Assert - Should return ViewResult with correct view name (Spring: view().name("vets/vetList"))
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("VetList", viewResult.ViewName);
            
            // Verify model attributes exist (Spring: model().attributeExists("listVets"))
            Assert.NotNull(viewResult.ViewData["ListVets"]);
            var listVets = Assert.IsType<List<Vet>>(viewResult.ViewData["ListVets"]);
            Assert.Equal(2, listVets.Count);
            Assert.Equal("James", listVets[0].FirstName);
            Assert.Equal("Helen", listVets[1].FirstName);
        }

        /// <summary>
        /// Spring: testShowResourcesVetList
        /// Tests GET /vets with JSON Accept header - should return JSON with vetList property
        /// </summary>
        [Fact]
        public async Task TestShowResourcesVetList()
        {
            // Arrange - Set up mock repository
            var vets = new List<Vet> { CreateJames(), CreateHelen() };
            _mockVetRepository.Setup(r => r.FindAllAsync())
                             .ReturnsAsync(vets);

            // Act - Call the JSON endpoint (Spring: get("/vets").accept(MediaType.APPLICATION_JSON))
            var result = await _controller.ShowResourcesVetList();

            // Assert - Should return JsonResult with Vets wrapper (Spring: jsonPath("$.vetList[0].id").value(1))
            var jsonResult = Assert.IsType<JsonResult>(result);
            var vetsResponse = Assert.IsType<Vets>(jsonResult.Value);
            
            // Verify the response structure matches Spring expectation: { "vetList": [vets] }
            Assert.NotNull(vetsResponse.VetList);
            Assert.Equal(2, vetsResponse.VetList.Count);
            
            // Verify the first vet has ID 1 (matches Spring: jsonPath("$.vetList[0].id").value(1))
            Assert.Equal(1, vetsResponse.VetList[0].Id);
            Assert.Equal("James", vetsResponse.VetList[0].FirstName);
        }
    }
}
