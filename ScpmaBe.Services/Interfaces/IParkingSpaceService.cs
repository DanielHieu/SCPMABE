using ScpmaBe.Services.Models;

namespace ScpmaBe.Services.Interfaces
{
    public interface IParkingSpaceService
    {
        Task<ParkingSpaceResponse> GetById(int id);
        Task<ParkingSpaceResponse> AddParkingSpaceAsync(AddParkingSpaceRequest request);
        Task<ParkingSpaceResponse> UpdateParkingSpaceAsync(UpdateParkingSpaceRequest request);
        Task<bool> DeleteParkingSpaceAsync(int id);

        Task<List<ParkingSpaceResponse>> GetParkingSpacesByFloor(int floorId);

        Task<bool> ChangeStatus(string apiKey, int value);

    }
}
