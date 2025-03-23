using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmBe.Repositories.Infrastructure;

namespace ScpmaBe.Repositories
{
    public class ParkingLotRepository : Repository<ParkingLot>, IParkingLotRepository
    {
        public ParkingLotRepository(SCPMContext _context) : base(_context) { }

        public override async Task<ParkingLot> GetById(int id)
        {
            return await Table.Include(x=>x.ParkingLotPriceHistories)
                              .FirstOrDefaultAsync(x => x.ParkingLotId == id);
        }
    }

}
