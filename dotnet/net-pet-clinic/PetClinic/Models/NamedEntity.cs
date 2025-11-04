using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetClinic.Models
{
    /// <summary>
    /// Simple domain object that adds a name property to BaseEntity. Used as
    /// a base class for objects needing these properties.
    /// 
    /// Ported from: org.springframework.samples.petclinic.model.NamedEntity
    /// </summary>
    public abstract class NamedEntity : BaseEntity
    {
        [Column("name")]
        [Required]
        public string Name { get; set; } = default!;

        public override string ToString()
        {
            return Name ?? string.Empty;
        }
    }
}
