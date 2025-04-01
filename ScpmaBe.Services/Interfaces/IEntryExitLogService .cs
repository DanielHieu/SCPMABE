using ScpmaBe.Repositories.Entities;
using ScpmaBe.Services.Models;

namespace ScpmaBe.Services.Interfaces
{
    public interface IEntryExitLogService
    {
        Task<SearchResultResponse<EntryExitLogResponse>> Search(SearchEntryExitLogRequest request);

        Task<EntryExitLog> GetById(int id);
        Task<EntryExitLog> AddEntryExitLogAsync(AddEntryExitLogRequest request);
        Task<bool> DeleteEntryExitLogAsync(int id);

        Task<CalculateFeeResponse> CalculateFeeAsync(CalculateFeeRequest request);
        Task<List<EntrancingCarResponse>> GetEntrancingCars(int parkingLotId);
        Task<bool> Pay(int id, string exitImage);
    }
}
