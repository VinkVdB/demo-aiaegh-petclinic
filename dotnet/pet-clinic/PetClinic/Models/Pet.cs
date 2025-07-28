using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetClinic.Models
{
    /// <summary>
    /// Simple business object representing a pet.
    /// 
    /// Ported from: org.springframework.samples.petclinic.owner.Pet
    /// </summary>
    [Table("pets")]
    public class Pet : NamedEntity
    {
        [Column("birth_date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Birth date is required.")]
        [PastDate(ErrorMessage = "Birth date cannot be in the future.")]
        public DateTime? BirthDate { get; set; }

        // Foreign key for PetType
        [Column("type_id")]
        [Required(ErrorMessage = "Pet type is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid pet type.")]
        public int? TypeId { get; set; }

        // Navigation property for PetType
        public virtual PetType? Type { get; set; }

        // Foreign key for Owner
        [Column("owner_id")]
        public int? OwnerId { get; set; }

        // Navigation property for Owner
        public virtual Owner? Owner { get; set; }

        // Navigation property for Visits (one-to-many)
        public virtual ICollection<Visit> Visits { get; set; } = new List<Visit>();

        public void AddVisit(Visit visit)
        {
            Visits.Add(visit);
            visit.Pet = this;
        }

        public IEnumerable<Visit> GetVisitsOrderedByDate()
        {
            return Visits.OrderBy(v => v.Date);
        }
    }
}
