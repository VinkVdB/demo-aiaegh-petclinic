using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetClinic.Models
{
    /// <summary>
    /// Simple domain object representing a veterinarian specialty (dentistry, surgery, etc.)
    /// 
    /// Ported from: org.springframework.samples.petclinic.vet.Specialty
    /// </summary>
    [Table("specialties")]
    public class Specialty : NamedEntity
    {
        // Inherits Id and Name from NamedEntity
        
        // Navigation property for many-to-many relationship with Vet
        public virtual ICollection<Vet> Vets { get; set; } = new List<Vet>();
    }
}
