using Microsoft.EntityFrameworkCore;
using PetClinic.Models;

namespace PetClinic.Data
{
    /// <summary>
    /// Entity Framework Core database context for PetClinic.
    /// 
    /// Ported from Spring Data JPA configuration
    /// </summary>
    public class PetClinicContext : DbContext
    {
        public PetClinicContext(DbContextOptions<PetClinicContext> options) : base(options)
        {
        }

        // DbSets for all entities
        public DbSet<Owner> Owners { get; set; } = default!;
        public DbSet<Pet> Pets { get; set; } = default!;
        public DbSet<PetType> PetTypes { get; set; } = default!;
        public DbSet<Vet> Vets { get; set; } = default!;
        public DbSet<Specialty> Specialties { get; set; } = default!;
        public DbSet<Visit> Visits { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Owner entity
            modelBuilder.Entity<Owner>(entity =>
            {
                entity.HasIndex(e => e.LastName).HasDatabaseName("owners_last_name");
                
                // Configure one-to-many relationship with Pet
                entity.HasMany(o => o.Pets)
                      .WithOne(p => p.Owner)
                      .HasForeignKey(p => p.OwnerId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Pet entity
            modelBuilder.Entity<Pet>(entity =>
            {
                // Configure many-to-one relationship with PetType
                entity.HasOne(p => p.Type)
                      .WithMany()
                      .HasForeignKey(p => p.TypeId);

                // Configure one-to-many relationship with Visit
                entity.HasMany(p => p.Visits)
                      .WithOne(v => v.Pet)
                      .HasForeignKey(v => v.PetId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure PetType entity
            modelBuilder.Entity<PetType>(entity =>
            {
                entity.HasIndex(e => e.Name).HasDatabaseName("types_name");
            });

            // Configure Vet entity
            modelBuilder.Entity<Vet>(entity =>
            {
                entity.HasIndex(e => e.LastName).HasDatabaseName("vets_last_name");

                // Configure many-to-many relationship with Specialty
                entity.HasMany(v => v.Specialties)
                      .WithMany(s => s.Vets)
                      .UsingEntity("vet_specialties",
                          l => l.HasOne(typeof(Specialty)).WithMany().HasForeignKey("specialty_id"),
                          r => r.HasOne(typeof(Vet)).WithMany().HasForeignKey("vet_id"));
            });

            // Configure Specialty entity
            modelBuilder.Entity<Specialty>(entity =>
            {
                entity.HasIndex(e => e.Name).HasDatabaseName("specialties_name");
            });

            // Configure Visit entity
            modelBuilder.Entity<Visit>(entity =>
            {
                // Visit already configured through Pet relationship
            });
        }
    }
}
