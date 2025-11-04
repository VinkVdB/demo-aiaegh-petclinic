using System.ComponentModel.DataAnnotations.Schema;

namespace PetClinic.Models
{
    /// <summary>
    /// Simple domain object representing a veterinarian.
    /// 
    /// Ported from: org.springframework.samples.petclinic.vet.Vet
    /// </summary>
    [Table("vets")]
    public class Vet : Person
    {
        // Navigation property for many-to-many relationship with Specialty
        public virtual ICollection<Specialty> Specialties { get; set; } = new List<Specialty>();

        public int GetNrOfSpecialties()
        {
            return Specialties.Count;
        }

        public void AddSpecialty(Specialty specialty)
        {
            Specialties.Add(specialty);
        }

        /// <summary>
        /// Gets specialties ordered by name (matching Spring version behavior)
        /// </summary>
        public IEnumerable<Specialty> GetSpecialtiesOrderedByName()
        {
            return Specialties.OrderBy(s => s.Name);
        }
    }
}
