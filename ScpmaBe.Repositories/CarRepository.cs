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
