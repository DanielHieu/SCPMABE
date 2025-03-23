using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmBe.Repositories.Infrastructure;

namespace ScpmaBe.Repositories
{
    public class StaffRepository : Repository<Staff>, IStaffRepository
    {
        public StaffRepository(SCPMContext _context) : base(_context) { }

        public override async Task<Staff> GetById(int id)
        {
            return await Table.FirstOrDefaultAsync(x => x.StaffId == id);
        }

        public async Task<bool> StaffIdExsistAsync(int id)
        {
            return await Table.AnyAsync(y => y.StaffId == id);
        }
    }
}
