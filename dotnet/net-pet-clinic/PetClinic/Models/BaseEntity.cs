using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetClinic.Models
{
    /// <summary>
    /// Simple domain object with an id property. Used as a base class for objects
    /// needing this property.
    /// 
    /// Ported from: org.springframework.samples.petclinic.model.BaseEntity
    /// </summary>
    public abstract class BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        public bool IsNew => Id == null;
    }
}
