using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmBe.Repositories.Infrastructure;

namespace ScpmaBe.Repositories
{
    public class FeedBackRepository : Repository<Feedback>, IFeedBackRepository
    {
        public FeedBackRepository(SCPMContext _context) : base(_context) { }

        public override async Task<Feedback> GetById(int id)
        {
            return await Table.FirstOrDefaultAsync(x => x.FeedbackId == id);
        }
    }
}
