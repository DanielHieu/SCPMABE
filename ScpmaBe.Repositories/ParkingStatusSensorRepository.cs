using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmBe.Repositories.Infrastructure;

namespace ScpmaBe.Repositories
{
    public class ParkingStatusSensorRepository : Repository<ParkingStatusSensor>, IParkingStatusSensorRepository
    {
        public ParkingStatusSensorRepository(SCPMContext dbContext) : base(dbContext)
        {
        }

        public override async Task<ParkingStatusSensor> GetById(int id)
        {
            return await GetAll().SingleOrDefaultAsync(x => x.ParkingStatusSensorId == id);
        }
    }
}
