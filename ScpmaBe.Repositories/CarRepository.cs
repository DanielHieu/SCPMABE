using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmBe.Repositories.Infrastructure;

namespace ScpmaBe.Repositories
{
    public class CarRepository : Repository<Car>, ICarRepository
    {
        public CarRepository(SCPMContext _context) : base(_context) { }

        public override async Task<Car> GetById(int id)
        {
            return await Table.FirstOrDefaultAsync(x => x.CarId == id);
        }

        public async Task<bool> CarIdExsistAsync(int carid)
        {
            return await Table.AnyAsync(x => x.CarId == carid);
        }
    }
}
