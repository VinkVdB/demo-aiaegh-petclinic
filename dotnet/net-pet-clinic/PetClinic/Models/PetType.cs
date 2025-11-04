using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetClinic.Models
{
    /// <summary>
    /// Simple domain object representing a pet type (Cat, Dog, Hamster, etc.)
    /// 
    /// Ported from: org.springframework.samples.petclinic.owner.PetType
    /// </summary>
    [Table("types")]
    public class PetType : NamedEntity
    {
        // Inherits Id and Name from NamedEntity
    }
}
