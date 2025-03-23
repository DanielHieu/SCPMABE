using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmBe.Repositories.Infrastructure;

namespace ScpmaBe.Repositories
{
    public class AssignedTaskRepository : Repository<AssignedTask>, IAssignedTaskRepository
    {
        public AssignedTaskRepository(SCPMContext _context) : base(_context) { }

        public override async Task<AssignedTask> GetById(int id)
        {
            return await Table.FirstOrDefaultAsync(x => x.AssignedTaskId == id);
        }
    }
}
