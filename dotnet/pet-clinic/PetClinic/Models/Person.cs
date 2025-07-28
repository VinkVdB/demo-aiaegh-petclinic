using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetClinic.Models
{
    /// <summary>
    /// Simple domain object representing a person.
    /// 
    /// Ported from: org.springframework.samples.petclinic.model.Person
    /// </summary>
    public abstract class Person : BaseEntity
    {
        [Column("first_name")]
        [Required]
        public string FirstName { get; set; } = default!;

        [Column("last_name")]
        [Required]
        public string LastName { get; set; } = default!;
    }
}
