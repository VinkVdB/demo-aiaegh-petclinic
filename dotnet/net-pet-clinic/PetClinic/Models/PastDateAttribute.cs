using System.ComponentModel.DataAnnotations;

namespace PetClinic.Models
{
    /// <summary>
    /// Custom validation attribute to ensure a date is not in the future
    /// </summary>
    public class PastDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null)
                return true; // Let [Required] handle null validation

            if (value is DateTime date)
            {
                return date <= DateTime.Now.Date;
            }

            return false;
        }
    }
}
