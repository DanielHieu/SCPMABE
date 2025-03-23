using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmBe.Repositories.Infrastructure;

namespace ScpmaBe.Repositories
{
    public class EntryExitLogRepository : Repository<EntryExitLog>, IEntryExitLogRepository
    {
        public EntryExitLogRepository(SCPMContext _context) : base(_context) { }

        public override async Task<EntryExitLog> GetById(int id)
        {
            return await Table.FirstOrDefaultAsync(x => x.EntryExitLogId == id);
        }
    }

}
