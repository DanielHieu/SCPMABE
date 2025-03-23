using ScpmaBe.Repositories.Entities;
using ScpmaBe.Services.Models;

namespace ScpmaBe.Services.Interfaces
{
    public interface IParkingLotService
    {
        Task<List<ParkingLot>> GetPaging(int pageIndex, int pageSize);
        Task<ParkingLot> GetById(int id);
        Task<ParkingLot> AddParkingLotAsync(AddParkingLotRequest request);
        Task<ParkingLot> UpdateParkingLotAsync(UpdateParkingLotRequest request);
        Task<bool> DeleteParkingLotAsync(int id);

        Task<List<ParkingLot>> Search(SearchParkingLotRequest request);
    }
}
