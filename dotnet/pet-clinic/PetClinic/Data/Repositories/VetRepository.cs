using Microsoft.EntityFrameworkCore;
using PetClinic.Models;

namespace PetClinic.Data.Repositories
{
    /// <summary>
    /// JPA implementation of the VetRepository interface.
    /// 
    /// Ported from: org.springframework.samples.petclinic.vet.VetRepository
    /// </summary>
    public class VetRepository : IVetRepository
    {
        private readonly PetClinicContext _context;

        public VetRepository(PetClinicContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Vet>> FindAllAsync()
        {
            return await _context.Vets
                .Include(v => v.Specialties)
                .OrderBy(v => v.LastName)
                .ToListAsync();
        }

        public async Task<Vet?> FindByIdAsync(int id)
        {
            return await _context.Vets
                .Include(v => v.Specialties)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<Vet> SaveAsync(Vet vet)
        {
            if (vet.IsNew)
            {
                _context.Vets.Add(vet);
            }
            else
            {
                _context.Vets.Update(vet);
            }
            
            await _context.SaveChangesAsync();
            return vet;
        }

        public async Task DeleteAsync(Vet vet)
        {
            _context.Vets.Remove(vet);
            await _context.SaveChangesAsync();
        }
    }
}
