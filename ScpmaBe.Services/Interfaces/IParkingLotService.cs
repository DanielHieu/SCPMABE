using ScpmaBe.Repositories.Entities;
using ScpmaBe.Services.Models;

namespace ScpmaBe.Services.Interfaces
{
    public interface IParkingLotService
    {
        Task<ParkingLot> GetById(int id);
        Task<ParkingLot> AddParkingLotAsync(AddParkingLotRequest request);
        Task<ParkingLot> UpdateParkingLotAsync(UpdateParkingLotRequest request);
        Task<bool> DeleteParkingLotAsync(int id);

        Task<List<ParkingLotResponse>> Search(SearchParkingLotRequest request);

        Task<ParkingLotFullResponse> GetFull(int parkingLotId);
        Task<ParkingLotSummaryStatusesResponse> GetSummaryStatuses(int parkingLotId);
        Task<ParkingLotSummaryResponse> GetSummaries(int parkingLotId);
        Task<IList<ParkingLotResponse>> GetAvailablesForContract(SearchAvailablesForContractRequest request);
        Task<bool> CheckEntranceExisted(string licensePlate);
        Task<ParkingLotStatsResponse> GetStats(int id);
    }
}
