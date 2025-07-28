using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetClinic.Models
{
    /// <summary>
    /// Simple domain object representing an owner.
    /// 
    /// Ported from: org.springframework.samples.petclinic.owner.Owner
    /// </summary>
    [Table("owners")]
    public class Owner : Person
    {
        [Column("address")]
        [Required]
        public string Address { get; set; } = default!;

        [Column("city")]
        [Required]
        public string City { get; set; } = default!;

        [Column("telephone")]
        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Telephone must be exactly 10 digits.")]
        public string Telephone { get; set; } = default!;

        // Navigation property for Pets (one-to-many)
        public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();

        public void AddPet(Pet pet)
        {
            if (pet.IsNew)
            {
                Pets.Add(pet);
                pet.Owner = this;
            }
        }

        /// <summary>
        /// Return the Pet with the given name, or null if none found for this Owner.
        /// </summary>
        /// <param name="name">The pet name to search for</param>
        /// <returns>The pet if found, null otherwise</returns>
        public Pet? GetPet(string name)
        {
            return GetPet(name, ignoreNew: false);
        }

        /// <summary>
        /// Return the Pet with the given name, or null if none found for this Owner.
        /// </summary>
        /// <param name="name">The pet name to search for</param>
        /// <param name="ignoreNew">Whether to ignore new pets (not yet persisted)</param>
        /// <returns>The pet if found, null otherwise</returns>
        public Pet? GetPet(string name, bool ignoreNew)
        {
            name = name.ToLowerInvariant();
            foreach (var pet in Pets)
            {
                if (!ignoreNew || !pet.IsNew)
                {
                    var compName = pet.Name?.ToLowerInvariant();
                    if (compName == name)
                    {
                        return pet;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Return the Pet with the given ID, or null if none found for this Owner.
        /// </summary>
        /// <param name="id">The pet ID to search for</param>
        /// <returns>The pet if found, null otherwise</returns>
        public Pet? GetPet(int id)
        {
            foreach (var pet in Pets)
            {
                if (pet.Id == id)
                {
                    return pet;
                }
            }
            return null;
        }

        /// <summary>
        /// Adds a visit to a specific pet
        /// </summary>
        /// <param name="petId">The pet ID</param>
        /// <param name="visit">The visit to add</param>
        public void AddVisit(int petId, Visit visit)
        {
            var pet = GetPet(petId);
            if (pet != null)
            {
                pet.AddVisit(visit);
            }
        }

        /// <summary>
        /// Gets pets ordered by name
        /// </summary>
        public IEnumerable<Pet> GetPetsOrderedByName()
        {
            return Pets.OrderBy(p => p.Name);
        }
    }
}
