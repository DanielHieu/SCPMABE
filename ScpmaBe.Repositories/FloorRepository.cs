using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmBe.Repositories.Infrastructure;

namespace ScpmaBe.Repositories
{
    public class FloorRepository : Repository<Floor>, IFloorRepository
    {
        public FloorRepository(SCPMContext _context) : base(_context) { }

        public override async Task<Floor> GetById(int id)
        {
            return await Table.FirstOrDefaultAsync(x => x.FloorId == id);
        }
    }

}
