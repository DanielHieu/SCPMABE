using ScpmaBe.Services.Models;

namespace ScpmaBe.Services.Interfaces
{
    public  interface  IAreaService
    {
        Task<AreaResponse> GetById(int id);
        Task<AreaResponse> AddAreaAsync(AddAreaRequest request);
        Task<AreaResponse> UpdateAreaAsync(UpdateAreaRequest request);
        Task<bool> DeleteAreaAsync(int id);
        Task<List<AreaResponse>> GetAreasByParkingLot(int parkingLotId);
    }
}
