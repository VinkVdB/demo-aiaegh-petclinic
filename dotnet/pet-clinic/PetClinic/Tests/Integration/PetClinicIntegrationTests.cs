using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http;
using Xunit;
using PetClinic.Data;
using PetClinic.Data.Repositories;
using PetClinic.Models;

namespace PetClinic.Tests.Integration;

/// <summary>
/// Integration tests for the PetClinic application
/// 
/// Migrated from Spring PetClinic PetClinicIntegrationTests.java
/// </summary>
public class PetClinicIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public PetClinicIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                // Replace the database with in-memory database for testing
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<PetClinicContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<PetClinicContext>(options =>
                {
                    options.UseInMemoryDatabase("IntegrationTestDb");
                });
            });
            
            // Configure the test app to handle errors gracefully (not in development mode)
            builder.Configure(app =>
            {
                app.UseExceptionHandler("/Error");  // Handle exceptions gracefully in tests
                app.UseStaticFiles();  // Enable static files in test mode
                app.UseRouting();
                app.UseAuthorization();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapDefaultControllerRoute();
                });
            });
        });
        
        // Seed data after creating client
        _client = _factory.CreateClient();
        
        // Initialize database
        EnsureDatabaseSeeded();
    }

    private void EnsureDatabaseSeeded()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PetClinicContext>();
        context.Database.EnsureCreated();
        SeedTestData(context);
    }

    private static void SeedTestData(PetClinicContext context)
    {
        if (context.PetTypes.Any()) return; // Already seeded

        // Add same test data as in ApplicationDbInitializer
        var petTypes = new[]
        {
            new PetType { Name = "cat" },
            new PetType { Name = "dog" },
            new PetType { Name = "lizard" },
            new PetType { Name = "snake" },
            new PetType { Name = "bird" },
            new PetType { Name = "hamster" }
        };

        context.PetTypes.AddRange(petTypes);

        var specialties = new[]
        {
            new Specialty { Name = "radiology" },
            new Specialty { Name = "surgery" },
            new Specialty { Name = "dentistry" }
        };

        context.Specialties.AddRange(specialties);

        var vets = new[]
        {
            new Vet { FirstName = "James", LastName = "Carter" },
            new Vet { FirstName = "Helen", LastName = "Leary" },
            new Vet { FirstName = "Linda", LastName = "Douglas" },
            new Vet { FirstName = "Rafael", LastName = "Ortega" },
            new Vet { FirstName = "Henry", LastName = "Stevens" },
            new Vet { FirstName = "Sharon", LastName = "Jenkins" }
        };

        context.Vets.AddRange(vets);

        var owners = new[]
        {
            new Owner { FirstName = "George", LastName = "Franklin", Address = "110 W. Liberty St.", City = "Madison", Telephone = "6085551023" },
            new Owner { FirstName = "Betty", LastName = "Davis", Address = "638 Cardinal Ave.", City = "Sun Prairie", Telephone = "6085551749" },
            new Owner { FirstName = "Eduardo", LastName = "Rodriquez", Address = "2693 Commerce St.", City = "McFarland", Telephone = "6085558763" },
            new Owner { FirstName = "Harold", LastName = "Davis", Address = "563 Friendly St.", City = "Windsor", Telephone = "6085553198" },
            new Owner { FirstName = "Peter", LastName = "McTavish", Address = "2387 S. Fair Way", City = "Madison", Telephone = "6085552765" },
            new Owner { FirstName = "Jean", LastName = "Coleman", Address = "105 N. Lake St.", City = "Monona", Telephone = "6085552654" },
            new Owner { FirstName = "Jeff", LastName = "Black", Address = "1450 Oak Blvd.", City = "Monona", Telephone = "6085555387" },
            new Owner { FirstName = "Maria", LastName = "Escobito", Address = "345 Maple St.", City = "Madison", Telephone = "6085557683" },
            new Owner { FirstName = "David", LastName = "Schroeder", Address = "2749 Blackhawk Trail", City = "Madison", Telephone = "6085559435" },
            new Owner { FirstName = "Carlos", LastName = "Estaban", Address = "2335 Independence La.", City = "Waunakee", Telephone = "6085555487" }
        };

        context.Owners.AddRange(owners);
        
        // Save everything first
        context.SaveChanges();

        // Now add pets - requires owners to exist first
        var dog = petTypes.First(pt => pt.Name == "dog");
        var cat = petTypes.First(pt => pt.Name == "cat");
        
        var pets = new[]
        {
            new Pet { Name = "Leo", BirthDate = new DateTime(2010, 9, 7), Type = dog, Owner = owners[0] },
            new Pet { Name = "Basil", BirthDate = new DateTime(2012, 8, 6), Type = cat, Owner = owners[1] },
            new Pet { Name = "Rosy", BirthDate = new DateTime(2011, 4, 17), Type = dog, Owner = owners[2] },
            new Pet { Name = "Jewel", BirthDate = new DateTime(2010, 3, 7), Type = dog, Owner = owners[2] }
        };

        context.Pets.AddRange(pets);
        context.SaveChanges();
    }

    [Fact]
    public async Task TestFindAll()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var vetRepository = scope.ServiceProvider.GetRequiredService<IVetRepository>();
        var context = scope.ServiceProvider.GetRequiredService<PetClinicContext>();

        // Debug: Check if data exists
        var vetsInDb = context.Vets.ToList();
        var ownersInDb = context.Owners.ToList();
        
        // Act & Assert - Test repository caching behavior
        var vets1 = await vetRepository.FindAllAsync();
        var vets2 = await vetRepository.FindAllAsync(); // Should be served from cache if caching is enabled

        Assert.NotNull(vets1);
        Assert.NotNull(vets2);
        Assert.True(vetsInDb.Count > 0, $"No vets in database. Found {vetsInDb.Count} vets and {ownersInDb.Count} owners");
        Assert.True(vets1.Any(), $"Repository returned no vets. DB has {vetsInDb.Count} vets");
        Assert.True(vets2.Any());
    }

    [Fact]
    public async Task TestOwnerDetails()
    {
        // Act
        var response = await _client.GetAsync("/owners/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
