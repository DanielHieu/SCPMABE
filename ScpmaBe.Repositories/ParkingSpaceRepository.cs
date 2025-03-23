using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmBe.Repositories.Infrastructure;

namespace ScpmaBe.Repositories
{
    public class ParkingSpaceRepository : Repository<ParkingSpace>, IParkingSpaceRepository
    {
        public ParkingSpaceRepository(SCPMContext dbContext) : base(dbContext)
        {
        }

        public override async Task<ParkingSpace> GetById(int id)
        {
            return await Table.FirstOrDefaultAsync(x => x.ParkingSpaceId == id);
        }

        public async Task<bool> ParkingSpaceIdExsistAsync(int id)
        {
            return await Table.AnyAsync(x => x.ParkingSpaceId == id);
        }
    }
}
