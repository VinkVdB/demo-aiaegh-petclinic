using PetClinic.Models;

namespace PetClinic.Data.Repositories
{
    /// <summary>
    /// Repository interface for PetType entities.
    /// 
    /// Ported from: org.springframework.samples.petclinic.owner.PetTypeRepository
    /// </summary>
    public interface IPetTypeRepository
    {
        /// <summary>
        /// Retrieve all PetTypes from the data store.
        /// </summary>
        /// <returns>All pet types</returns>
        Task<IEnumerable<PetType>> FindAllAsync();

        /// <summary>
        /// Retrieve a PetType from the data store by id.
        /// </summary>
        /// <param name="id">The id to search for</param>
        /// <returns>The pet type if found, null otherwise</returns>
        Task<PetType?> FindByIdAsync(int id);

        /// <summary>
        /// Save a PetType to the data store, either inserting or updating.
        /// </summary>
        /// <param name="petType">The pet type to save</param>
        /// <returns>The saved pet type</returns>
        Task<PetType> SaveAsync(PetType petType);

        /// <summary>
        /// Delete a PetType from the data store.
        /// </summary>
        /// <param name="petType">The pet type to delete</param>
        Task DeleteAsync(PetType petType);
    }
}
