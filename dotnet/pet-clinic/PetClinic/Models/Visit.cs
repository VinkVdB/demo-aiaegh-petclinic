using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetClinic.Models
{
    /// <summary>
    /// Simple domain object representing a visit.
    /// 
    /// Ported from: org.springframework.samples.petclinic.owner.Visit
    /// </summary>
    [Table("visits")]
    public class Visit : BaseEntity
    {
        [Column("visit_date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Visit date is required.")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(255, ErrorMessage = "Description cannot be longer than 255 characters.")]
        public string Description { get; set; } = default!;

        // Foreign key for Pet
        [Column("pet_id")]
        public int? PetId { get; set; }

        // Navigation property
        public virtual Pet? Pet { get; set; }

        /// <summary>
        /// Creates a new instance of Visit for the current date
        /// </summary>
        public Visit()
        {
            Date = DateTime.Today;
        }
    }
}
