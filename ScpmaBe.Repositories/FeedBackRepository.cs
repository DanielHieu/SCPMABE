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
    public class FeedBackRepository : Repository<Feedback>, IFeedBackRepository
    {
        public FeedBackRepository(SCPMContext _context) : base(_context) { }

        public override async Task<Feedback> GetById(int id)
        {
            return await Table.FirstOrDefaultAsync(x => x.FeedbackId == id);
        }
    }
}
