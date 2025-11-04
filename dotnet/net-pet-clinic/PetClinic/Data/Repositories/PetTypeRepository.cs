using Microsoft.EntityFrameworkCore;
using PetClinic.Models;

namespace PetClinic.Data.Repositories
{
    /// <summary>
    /// JPA implementation of the PetTypeRepository interface.
    /// 
    /// Ported from: org.springframework.samples.petclinic.owner.PetTypeRepository
    /// </summary>
    public class PetTypeRepository : IPetTypeRepository
    {
        private readonly PetClinicContext _context;

        public PetTypeRepository(PetClinicContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PetType>> FindAllAsync()
        {
            return await _context.PetTypes
                .OrderBy(pt => pt.Name)
                .ToListAsync();
        }

        public async Task<PetType?> FindByIdAsync(int id)
        {
            return await _context.PetTypes
                .FirstOrDefaultAsync(pt => pt.Id == id);
        }

        public async Task<PetType> SaveAsync(PetType petType)
        {
            if (petType.IsNew)
            {
                _context.PetTypes.Add(petType);
            }
            else
            {
                _context.PetTypes.Update(petType);
            }
            
            await _context.SaveChangesAsync();
            return petType;
        }

        public async Task DeleteAsync(PetType petType)
        {
            _context.PetTypes.Remove(petType);
            await _context.SaveChangesAsync();
        }
    }
}
