namespace PetClinic.Models
{
    /// <summary>
    /// Simple domain object representing a list of veterinarians. 
    /// Used for JSON/XML serialization in API responses.
    /// 
    /// Ported from: org.springframework.samples.petclinic.vet.Vets
    /// </summary>
    public class Vets
    {
        public List<Vet> VetList { get; set; } = new List<Vet>();
    }
}
