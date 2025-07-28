using PetClinic.Models;

namespace PetClinic.Data.Repositories
{
    /// <summary>
    /// Repository interface for Owner entities.
    /// 
    /// Ported from: org.springframework.samples.petclinic.owner.OwnerRepository
    /// </summary>
    public interface IOwnerRepository
    {
        /// <summary>
        /// Retrieve all Owners from the data store.
        /// </summary>
        /// <returns>All owners</returns>
        Task<IEnumerable<Owner>> FindAllAsync();

        /// <summary>
        /// Retrieve Owners from the data store by last name, returning all owners
        /// whose last name starts with the given name.
        /// </summary>
        /// <param name="lastName">Value to search for</param>
        /// <param name="page">Page number (0-based)</param>
        /// <param name="size">Page size</param>
        /// <returns>A collection of matching Owners</returns>
        Task<(IEnumerable<Owner> Owners, int TotalCount)> FindByLastNameStartingWithAsync(string lastName, int page, int size);

        /// <summary>
        /// Retrieve an Owner from the data store by id.
        /// </summary>
        /// <param name="id">The id to search for</param>
        /// <returns>The owner if found, null otherwise</returns>
        Task<Owner?> FindByIdAsync(int id);

        /// <summary>
        /// Save an Owner to the data store, either inserting or updating.
        /// </summary>
        /// <param name="owner">The owner to save</param>
        /// <returns>The saved owner</returns>
        Task<Owner> SaveAsync(Owner owner);

        /// <summary>
        /// Delete an Owner from the data store.
        /// </summary>
        /// <param name="owner">The owner to delete</param>
        Task DeleteAsync(Owner owner);
    }
}
