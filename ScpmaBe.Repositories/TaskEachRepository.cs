using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmBe.Repositories.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
