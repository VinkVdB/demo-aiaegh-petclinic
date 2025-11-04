using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetClinic.Data;
using PetClinic.Data.Repositories;
using PetClinic.Models;
using Xunit;

namespace PetClinic.Tests.Integration
{
    /// <summary>
    /// Integration test of the Repository layer with real database operations.
    /// 
    /// Migrated from: org.springframework.samples.petclinic.service.ClinicServiceTests
    /// 
    /// These tests use an in-memory SQLite database to verify repository implementations
    /// work correctly with actual database operations, matching Spring's @DataJpaTest approach.
    /// </summary>
    public class ClinicServiceTests : IDisposable
    {
        private readonly PetClinicContext _context;
        private readonly IOwnerRepository _owners;
        private readonly IPetTypeRepository _types;
        private readonly IVetRepository _vets;
        private readonly IServiceProvider _serviceProvider;

        public ClinicServiceTests()
        {
            // Set up in-memory database for integration testing
            var services = new ServiceCollection();
            services.AddDbContext<PetClinicContext>(options =>
                options.UseSqlite("Data Source=:memory:"));
            services.AddScoped<IOwnerRepository, OwnerRepository>();
            services.AddScoped<IPetTypeRepository, PetTypeRepository>();
            services.AddScoped<IVetRepository, VetRepository>();

            _serviceProvider = services.BuildServiceProvider();
            _context = _serviceProvider.GetRequiredService<PetClinicContext>();
            _owners = _serviceProvider.GetRequiredService<IOwnerRepository>();
            _types = _serviceProvider.GetRequiredService<IPetTypeRepository>();
            _vets = _serviceProvider.GetRequiredService<IVetRepository>();

            // Initialize database
            _context.Database.OpenConnection();
            _context.Database.EnsureCreated();
            SeedTestData();
        }

        /// <summary>
        /// Seed the database with test data matching Spring's dataset
        /// </summary>
        private void SeedTestData()
        {
            // Add PetTypes (matches Spring test expectations)
            var catType = new PetType { Name = "cat" };
            var dogType = new PetType { Name = "dog" };
            var snakeType = new PetType { Name = "snake" };
            var birdType = new PetType { Name = "bird" };
            _context.PetTypes.AddRange(catType, dogType, snakeType, birdType);

            // Add Specialties for vets
            var dentistry = new Specialty { Name = "dentistry" };
            var surgery = new Specialty { Name = "surgery" };
            _context.Specialties.AddRange(dentistry, surgery);

            // Add test owners (Spring test expects owners with "Davis" and "Franklin")
            var owner1 = new Owner
            {
                FirstName = "George",
                LastName = "Franklin",
                Address = "110 W. Liberty St.",
                City = "Madison", 
                Telephone = "6085551023"
            };

            var owner2 = new Owner
            {
                FirstName = "Betty",
                LastName = "Davis",
                Address = "638 Cardinal Ave.",
                City = "Sun Prairie",
                Telephone = "6085551749"
            };

            var owner3 = new Owner
            {
                FirstName = "Eduardo",
                LastName = "Davis", 
                Address = "2693 Commerce St.",
                City = "McFarland",
                Telephone = "6085558763"
            };

            var owner6 = new Owner
            {
                FirstName = "Jean",
                LastName = "Coleman",
                Address = "105 N. Lake St.",
                City = "Monona",
                Telephone = "6085552654"
            };

            _context.Owners.AddRange(owner1, owner2, owner3, owner6);
            _context.SaveChanges();

            // Add pets (Spring test expects owner1 to have a cat)
            var pet1 = new Pet
            {
                Name = "Leo",
                BirthDate = DateTime.Parse("2010-09-07"),
                Type = catType,
                Owner = owner1
            };

            var pet7 = new Pet
            {
                Name = "Samantha", 
                BirthDate = DateTime.Parse("2012-09-04"),
                Type = catType,
                Owner = owner6
            };

            owner1.AddPet(pet1);
            owner6.AddPet(pet7);
            _context.SaveChanges();

            // Add visits (Spring test expects pet7 to have 2 visits)
            var visit1 = new Visit
            {
                Date = DateTime.Parse("2013-01-01"),
                Description = "rabies shot",
                Pet = pet7
            };

            var visit2 = new Visit
            {
                Date = DateTime.Parse("2013-01-02"), 
                Description = "neutered",
                Pet = pet7
            };

            pet7.Visits.Add(visit1);
            pet7.Visits.Add(visit2);

            // Add vets (Spring test expects vet with ID 3 to be "Douglas" with 2 specialties)
            var vet3 = new Vet
            {
                FirstName = "Linda",
                LastName = "Douglas"
            };
            vet3.AddSpecialty(dentistry);
            vet3.AddSpecialty(surgery);
            _context.Vets.Add(vet3);

            _context.SaveChanges();
        }

        /// <summary>
        /// Spring: shouldFindOwnersByLastName
        /// Tests owner search by last name with pagination
        /// </summary>
        [Fact]
        public async Task ShouldFindOwnersByLastName()
        {
            // Act & Assert - Should find 2 owners with "Davis" (matches Spring test)
            var davises = await _owners.FindByLastNameStartingWithAsync("Davis", 0, 10);
            Assert.Equal(2, davises.TotalCount);

            // Should find 0 owners with "Daviss" (typo)
            var noMatch = await _owners.FindByLastNameStartingWithAsync("Daviss", 0, 10);
            Assert.Equal(0, noMatch.TotalCount);
        }

        /// <summary>
        /// Spring: shouldFindSingleOwnerWithPet
        /// Tests finding owner by ID with pet relationships loaded
        /// </summary>
        [Fact]
        public async Task ShouldFindSingleOwnerWithPet()
        {
            // Act - Find owner by ID 1 (matches Spring test)
            var owner = await _owners.FindByIdAsync(1);

            // Assert - Should be Franklin with 1 cat (matches Spring expectations)
            Assert.NotNull(owner);
            Assert.StartsWith("Franklin", owner.LastName);
            Assert.Single(owner.Pets);
            Assert.NotNull(owner.Pets.First().Type);
            Assert.Equal("cat", owner.Pets.First().Type.Name);
        }

        /// <summary>
        /// Spring: shouldInsertOwner
        /// Tests inserting new owner and verifying database changes
        /// </summary>
        [Fact]
        public async Task ShouldInsertOwner()
        {
            // Arrange - Count existing "Schultz" owners
            var initialSchultzes = await _owners.FindByLastNameStartingWithAsync("Schultz", 0, 10);
            var initialCount = initialSchultzes.TotalCount;

            // Create new owner (matches Spring test data)
            var owner = new Owner
            {
                FirstName = "Sam",
                LastName = "Schultz",
                Address = "4, Evans Street",
                City = "Wollongong",
                Telephone = "4444444444"
            };

            // Act - Save owner
            var savedOwner = await _owners.SaveAsync(owner);

            // Assert - Owner should have generated ID
            Assert.True(savedOwner.Id.HasValue && savedOwner.Id > 0);

            // Verify count increased by 1
            var finalSchultzes = await _owners.FindByLastNameStartingWithAsync("Schultz", 0, 10);
            Assert.Equal(initialCount + 1, finalSchultzes.TotalCount);
        }

        /// <summary>
        /// Spring: shouldUpdateOwner
        /// Tests updating existing owner data
        /// </summary>
        [Fact]
        public async Task ShouldUpdateOwner()
        {
            // Arrange - Get owner 1 and modify name
            var owner = await _owners.FindByIdAsync(1);
            Assert.NotNull(owner);
            var oldLastName = owner.LastName;
            var newLastName = oldLastName + "X";

            // Act - Update owner
            owner.LastName = newLastName;
            await _owners.SaveAsync(owner);

            // Assert - Verify change persisted
            var updatedOwner = await _owners.FindByIdAsync(1);
            Assert.NotNull(updatedOwner);
            Assert.Equal(newLastName, updatedOwner.LastName);
        }

        /// <summary>
        /// Spring: shouldFindAllPetTypes
        /// Tests retrieving all pet types
        /// </summary>
        [Fact]
        public async Task ShouldFindAllPetTypes()
        {
            // Act
            var petTypes = await _types.FindAllAsync();
            var typesList = petTypes.ToList();

            // Assert - Should find expected pet types (matches Spring test)
            Assert.True(typesList.Count >= 4);
            
            var catType = typesList.FirstOrDefault(t => t.Id == 1);
            Assert.NotNull(catType);
            Assert.Equal("cat", catType.Name);
            
            var snakeType = typesList.FirstOrDefault(t => t.Name == "snake");
            Assert.NotNull(snakeType);
            Assert.Equal("snake", snakeType.Name);
        }

        /// <summary>
        /// Spring: shouldInsertPetIntoDatabaseAndGenerateId
        /// Tests adding new pet to existing owner
        /// </summary>
        [Fact]
        public async Task ShouldInsertPetIntoDatabaseAndGenerateId()
        {
            // Arrange - Get owner 6 and count existing pets
            var owner6 = await _owners.FindByIdAsync(4); // Owner6 is at index 4 in our seeded data
            Assert.NotNull(owner6);
            var initialPetCount = owner6.Pets.Count;

            // Create new pet (matches Spring test data)
            var pet = new Pet
            {
                Name = "bowser",
                BirthDate = DateTime.Now.Date
            };
            
            var types = await _types.FindAllAsync();
            var dogType = types.FirstOrDefault(t => t.Name == "dog");
            Assert.NotNull(dogType);
            pet.Type = dogType;

            // Act - Add pet to owner
            owner6.AddPet(pet);
            Assert.Equal(initialPetCount + 1, owner6.Pets.Count);

            await _owners.SaveAsync(owner6);

            // Assert - Verify pet was saved with generated ID
            var updatedOwner6 = await _owners.FindByIdAsync(4);
            Assert.NotNull(updatedOwner6);
            Assert.Equal(initialPetCount + 1, updatedOwner6.Pets.Count);
            
            var bowser = updatedOwner6.GetPet("bowser");
            Assert.NotNull(bowser);
            Assert.True(bowser.Id.HasValue);
        }

        /// <summary>
        /// Spring: shouldUpdatePetName
        /// Tests updating pet name
        /// </summary>
        [Fact]
        public async Task ShouldUpdatePetName()
        {
            // Arrange - Get owner 6 and pet 7 equivalent
            var owner6 = await _owners.FindByIdAsync(4);
            Assert.NotNull(owner6);
            var pet7 = owner6.Pets.FirstOrDefault();
            Assert.NotNull(pet7);
            
            var oldName = pet7.Name;
            var newName = oldName + "X";

            // Act - Update pet name
            pet7.Name = newName;
            await _owners.SaveAsync(owner6);

            // Assert - Verify change persisted
            var updatedOwner6 = await _owners.FindByIdAsync(4);
            Assert.NotNull(updatedOwner6);
            var updatedPet = updatedOwner6.Pets.FirstOrDefault(p => p.Name == newName);
            Assert.NotNull(updatedPet);
            Assert.Equal(newName, updatedPet.Name);
        }

        /// <summary>
        /// Spring: shouldFindVets
        /// Tests finding vets with specialties
        /// </summary>
        [Fact]
        public async Task ShouldFindVets()
        {
            // Act
            var vets = await _vets.FindAllAsync();
            var vetsList = vets.ToList();

            // Assert - Find vet 3 "Douglas" with 2 specialties (matches Spring test)
            var douglas = vetsList.FirstOrDefault(v => v.LastName == "Douglas");
            Assert.NotNull(douglas);
            Assert.Equal("Douglas", douglas.LastName);
            Assert.Equal(2, douglas.Specialties.Count);
            Assert.Contains(douglas.Specialties, s => s.Name == "dentistry");
            Assert.Contains(douglas.Specialties, s => s.Name == "surgery");
        }

        /// <summary>
        /// Spring: shouldAddNewVisitForPet
        /// Tests adding visit to existing pet
        /// </summary>
        [Fact]
        public async Task ShouldAddNewVisitForPet()
        {
            // Arrange - Get owner 6 and pet 7 equivalent
            var owner6 = await _owners.FindByIdAsync(4);
            Assert.NotNull(owner6);
            var pet7 = owner6.Pets.FirstOrDefault();
            Assert.NotNull(pet7);
            
            var initialVisitCount = pet7.Visits.Count;

            // Create new visit (matches Spring test)
            var visit = new Visit
            {
                Description = "test",
                Date = DateTime.Now.Date
            };

            // Act - Add visit
            owner6.AddVisit(pet7.Id!.Value, visit);
            await _owners.SaveAsync(owner6);

            // Assert - Verify visit was added with generated ID
            var updatedOwner6 = await _owners.FindByIdAsync(4);
            Assert.NotNull(updatedOwner6);
            var updatedPet7 = updatedOwner6.Pets.FirstOrDefault();
            Assert.NotNull(updatedPet7);
            
            Assert.Equal(initialVisitCount + 1, updatedPet7.Visits.Count);
            Assert.All(updatedPet7.Visits, v => Assert.True(v.Id.HasValue));
        }

        /// <summary>
        /// Spring: shouldFindVisitsByPetId
        /// Tests retrieving visits for a specific pet
        /// </summary>
        [Fact]
        public async Task ShouldFindVisitsByPetId()
        {
            // Arrange - Get owner 6 and pet 7 equivalent
            var owner6 = await _owners.FindByIdAsync(4);
            Assert.NotNull(owner6);
            var pet7 = owner6.Pets.FirstOrDefault();
            Assert.NotNull(pet7);

            // Act & Assert - Should have 2 visits (from seeded data)
            var visits = pet7.Visits;
            Assert.Equal(2, visits.Count);
            Assert.All(visits, v => Assert.NotNull(v.Date));
        }

        public void Dispose()
        {
            _context?.Dispose();
            _serviceProvider?.GetService<IServiceScope>()?.Dispose();
        }
    }
}
