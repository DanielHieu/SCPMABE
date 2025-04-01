using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmBe.Repositories.Infrastructure;

namespace ScpmaBe.Repositories
{
    public class TaskEachRepository : Repository<TaskEach>, ITaskEachRepository
    {
        public TaskEachRepository(SCPMContext _context) : base(_context) { }

        public override async Task<TaskEach> GetById(int id)
        {
            return await Table.FirstOrDefaultAsync(x => x.TaskEachId == id);
        }

        public async Task<bool> TaskEachIdExsistAsync(int id)
        {
            return await Table.AnyAsync(y => y.TaskEachId == id);
        }
    }
}
