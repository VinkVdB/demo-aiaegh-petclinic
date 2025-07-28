using Microsoft.EntityFrameworkCore;
using PetClinic.Models;

namespace PetClinic.Data
{
    /// <summary>
    /// Database initializer that seeds sample data matching the Spring PetClinic version.
    /// 
    /// Ported from: src/main/resources/db/h2/data.sql
    /// </summary>
    public static class ApplicationDbInitializer
    {
        public static async Task SeedAsync(PetClinicContext context)
        {
            // Apply migrations to ensure database is up to date (only for relational databases)
            if (context.Database.IsRelational())
            {
                await context.Database.MigrateAsync();
            }
            else
            {
                // For in-memory databases, just ensure it's created
                await context.Database.EnsureCreatedAsync();
            }

            // Check if data already exists
            if (await context.Owners.AnyAsync())
            {
                return; // Database has been seeded
            }

            // Seed Vets
            var vets = new[]
            {
                new Vet { FirstName = "James", LastName = "Carter" },
                new Vet { FirstName = "Helen", LastName = "Leary" },
                new Vet { FirstName = "Linda", LastName = "Douglas" },
                new Vet { FirstName = "Rafael", LastName = "Ortega" },
                new Vet { FirstName = "Henry", LastName = "Stevens" },
                new Vet { FirstName = "Sharon", LastName = "Jenkins" }
            };
            await context.Vets.AddRangeAsync(vets);
            await context.SaveChangesAsync();

            // Seed Specialties
            var specialties = new[]
            {
                new Specialty { Name = "radiology" },
                new Specialty { Name = "surgery" },
                new Specialty { Name = "dentistry" }
            };
            await context.Specialties.AddRangeAsync(specialties);
            await context.SaveChangesAsync();

            // Seed Vet-Specialty relationships (matching original data.sql)
            vets[1].Specialties.Add(specialties[0]); // Helen Leary - radiology
            vets[2].Specialties.Add(specialties[1]); // Linda Douglas - surgery
            vets[2].Specialties.Add(specialties[2]); // Linda Douglas - dentistry
            vets[3].Specialties.Add(specialties[1]); // Rafael Ortega - surgery
            vets[4].Specialties.Add(specialties[0]); // Henry Stevens - radiology

            // Seed Pet Types
            var petTypes = new[]
            {
                new PetType { Name = "cat" },
                new PetType { Name = "dog" },
                new PetType { Name = "lizard" },
                new PetType { Name = "snake" },
                new PetType { Name = "bird" },
                new PetType { Name = "hamster" }
            };
            await context.PetTypes.AddRangeAsync(petTypes);
            await context.SaveChangesAsync();

            // Seed Owners
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
            await context.Owners.AddRangeAsync(owners);
            await context.SaveChangesAsync();

            // Seed Pets
            var pets = new[]
            {
                new Pet { Name = "Leo", BirthDate = new DateTime(2010, 9, 7), Type = petTypes[0], Owner = owners[0] },
                new Pet { Name = "Basil", BirthDate = new DateTime(2012, 8, 6), Type = petTypes[5], Owner = owners[1] },
                new Pet { Name = "Rosy", BirthDate = new DateTime(2011, 4, 17), Type = petTypes[1], Owner = owners[2] },
                new Pet { Name = "Jewel", BirthDate = new DateTime(2010, 3, 7), Type = petTypes[1], Owner = owners[2] },
                new Pet { Name = "Iggy", BirthDate = new DateTime(2010, 11, 30), Type = petTypes[2], Owner = owners[3] },
                new Pet { Name = "George", BirthDate = new DateTime(2010, 1, 20), Type = petTypes[3], Owner = owners[4] },
                new Pet { Name = "Samantha", BirthDate = new DateTime(2012, 9, 4), Type = petTypes[0], Owner = owners[5] },
                new Pet { Name = "Max", BirthDate = new DateTime(2012, 9, 4), Type = petTypes[0], Owner = owners[5] },
                new Pet { Name = "Lucky", BirthDate = new DateTime(2011, 8, 6), Type = petTypes[4], Owner = owners[6] },
                new Pet { Name = "Mulligan", BirthDate = new DateTime(2007, 2, 24), Type = petTypes[1], Owner = owners[7] },
                new Pet { Name = "Freddy", BirthDate = new DateTime(2010, 3, 9), Type = petTypes[4], Owner = owners[8] },
                new Pet { Name = "Lucky", BirthDate = new DateTime(2010, 6, 24), Type = petTypes[1], Owner = owners[9] },
                new Pet { Name = "Sly", BirthDate = new DateTime(2012, 6, 8), Type = petTypes[0], Owner = owners[9] }
            };
            await context.Pets.AddRangeAsync(pets);
            await context.SaveChangesAsync();

            // Seed Visits
            var visits = new[]
            {
                new Visit { Pet = pets[6], Date = new DateTime(2013, 1, 1), Description = "rabies shot" },    // Samantha
                new Visit { Pet = pets[7], Date = new DateTime(2013, 1, 2), Description = "rabies shot" },    // Max
                new Visit { Pet = pets[7], Date = new DateTime(2013, 1, 3), Description = "neutered" },       // Max
                new Visit { Pet = pets[6], Date = new DateTime(2013, 1, 4), Description = "spayed" }          // Samantha
            };
            await context.Visits.AddRangeAsync(visits);
            await context.SaveChangesAsync();

            Console.WriteLine("Database seeded with sample data matching Spring PetClinic");
        }
    }
}
