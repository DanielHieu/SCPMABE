using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmBe.Repositories.Infrastructure;

namespace ScpmaBe.Repositories
{
    public class AreaRepository : Repository<Area>, IAreaRepository
    {
        public AreaRepository(SCPMContext _context) : base(_context) { }

        public override async Task<Area> GetById(int id)
        {
            return await Table.FirstOrDefaultAsync(x => x.AreaId == id);
        }
    }

}
