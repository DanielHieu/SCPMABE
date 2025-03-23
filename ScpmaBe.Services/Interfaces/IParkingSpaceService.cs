using ScpmaBe.Repositories.Entities;
using ScpmaBe.Services.Models;

namespace ScpmaBe.Services.Interfaces
{
    public interface IParkingSpaceService
    {
        Task<List<ParkingSpace>> GetPaging(int pageIndex, int pageSize);
        Task<ParkingSpace> GetById(int id);
        Task<ParkingSpace> AddParkingSpaceAsync(AddParkingSpaceRequest request);
        Task<ParkingSpace> UpdateParkingSpaceAsync(UpdateParkingSpaceRequest request);
        Task<bool> DeleteParkingSpaceAsync(int id);

        Task<List<ParkingSpace>> GetParkingSpacesByFloor(int floorId);
    }
}
