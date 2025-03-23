using ScpmaBe.Repositories.Entities;
using ScpmaBe.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScpmaBe.Services.Interfaces
{
    public interface ICarService
    {
        Task<List<Car>> GetPaging(int pageIndex, int pageSize);

        Task<List<Car>> GetCarsOfCustomerAsync(int customerId);

        Task<List<Car>> GetCarsOfOwnerAsync(int ownerId);

        Task<Car> GetById(int id);

        Task<List<Car>> SearchCarAsync(SearchCarRequest request);

        Task<Car> AddCarAsync(AddCarRequest request);

        Task<Car> UpdateCarAsync(UpdateCarRequest request);

        Task<bool> DeleteCarAsync(int id);
    }
}
