using ScpmaBe.Repositories.Entities;
using ScpmaBe.Services.Models;

namespace ScpmaBe.Services.Interfaces
{
    public  interface IFloorService
    {
        Task<Floor> GetById(int id);
        Task<FloorResponse> AddFloorAsync(AddFloorRequest request);
        Task<FloorResponse> UpdateFloorAsync(UpdateFloorRequest request);
        Task<bool> DeleteFloorAsync(int id);

        Task<List<FloorResponse>> GetFloorsByArea(int areaId);
        Task<List<FloorResponse>> GetFloorsByParkingLot(int parkingLotId);
    }
}
