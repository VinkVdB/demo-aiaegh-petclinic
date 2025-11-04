using PetClinic.Models;

namespace PetClinic.Data.Repositories
{
    /// <summary>
    /// Repository interface for Vet entities.
    /// 
    /// Ported from: org.springframework.samples.petclinic.vet.VetRepository
    /// </summary>
    public interface IVetRepository
    {
        /// <summary>
        /// Retrieve all Vets from the data store including their specialties.
        /// </summary>
        /// <returns>All vets with specialties</returns>
        Task<IEnumerable<Vet>> FindAllAsync();

        /// <summary>
        /// Retrieve a Vet from the data store by id.
        /// </summary>
        /// <param name="id">The id to search for</param>
        /// <returns>The vet if found, null otherwise</returns>
        Task<Vet?> FindByIdAsync(int id);

        /// <summary>
        /// Save a Vet to the data store, either inserting or updating.
        /// </summary>
        /// <param name="vet">The vet to save</param>
        /// <returns>The saved vet</returns>
        Task<Vet> SaveAsync(Vet vet);

        /// <summary>
        /// Delete a Vet from the data store.
        /// </summary>
        /// <param name="vet">The vet to delete</param>
        Task DeleteAsync(Vet vet);
    }
}
