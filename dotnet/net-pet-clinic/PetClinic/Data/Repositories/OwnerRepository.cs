using Microsoft.EntityFrameworkCore;
using PetClinic.Models;

namespace PetClinic.Data.Repositories
{
    /// <summary>
    /// JPA implementation of the OwnerRepository interface.
    /// 
    /// Ported from: org.springframework.samples.petclinic.owner.OwnerRepository
    /// </summary>
    public class OwnerRepository : IOwnerRepository
    {
        private readonly PetClinicContext _context;

        public OwnerRepository(PetClinicContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Owner>> FindAllAsync()
        {
            return await _context.Owners
                .Include(o => o.Pets)
                    .ThenInclude(p => p.Type)
                .Include(o => o.Pets)
                    .ThenInclude(p => p.Visits)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Owner> Owners, int TotalCount)> FindByLastNameStartingWithAsync(string lastName, int page, int size)
        {
            var query = _context.Owners
                .Include(o => o.Pets)
                    .ThenInclude(p => p.Type)
                .Include(o => o.Pets)
                    .ThenInclude(p => p.Visits)
                .Where(o => o.LastName.StartsWith(lastName))
                .OrderBy(o => o.LastName);

            var totalCount = await query.CountAsync();
            var owners = await query
                .Skip((page - 1) * size)  // Convert 1-based page to 0-based index
                .Take(size)
                .ToListAsync();

            return (owners, totalCount);
        }

        public async Task<Owner?> FindByIdAsync(int id)
        {
            return await _context.Owners
                .Include(o => o.Pets)
                    .ThenInclude(p => p.Type)
                .Include(o => o.Pets)
                    .ThenInclude(p => p.Visits)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Owner> SaveAsync(Owner owner)
        {
            if (owner.IsNew)
            {
                _context.Owners.Add(owner);
            }
            else
            {
                _context.Owners.Update(owner);
            }
            
            await _context.SaveChangesAsync();
            return owner;
        }

        public async Task DeleteAsync(Owner owner)
        {
            _context.Owners.Remove(owner);
            await _context.SaveChangesAsync();
        }
    }
}
