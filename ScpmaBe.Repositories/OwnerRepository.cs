using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmBe.Repositories.Infrastructure;

namespace ScpmaBe.Repositories
{
    public class OwnerRepository : Repository<Owner>, IOwnerRepository
    {
        public OwnerRepository(SCPMContext _context) : base(_context) { }

        public override async Task<Owner> GetById(int id)
        {
            return await Table.FirstOrDefaultAsync(x => x.OwnerId == id);
        }

        public async Task<bool> ExistsByIdAsync(int ownerId)
        {
            return await Table.AnyAsync(o => o.OwnerId == ownerId);
        }
    }
}
