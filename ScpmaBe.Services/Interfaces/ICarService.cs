using ScpmaBe.Repositories.Entities;
using ScpmaBe.Services.Models;

namespace ScpmaBe.Services.Interfaces
{
    public interface ICarService
    {
        string GetImageFolder();
        Task<List<CarAndEntranceResponse>> GetCarsOfCustomerAsync(int customerId);

        Task<CarAndEntranceResponse> GetById(int id);

        Task<List<Car>> SearchCarAsync(SearchCarRequest request);

        Task<Car> AddCarAsync(AddCarRequest request);

        Task<Car> UpdateCarAsync(UpdateCarRequest request);

        Task<bool> DeleteCarAsync(int id);
    }
}
