using ScpmaBe.Repositories.Entities;
using ScpmaBe.Services.Models;

namespace ScpmaBe.Services.Interfaces
{
    public interface IEntryExitLogService
    {
        Task<List<EntryExitLog>> GetPaging(int pageIndex, int pageSize);
        Task<EntryExitLog> GetById(int id);
        Task<EntryExitLog> AddEntryExitLogAsync(AddEntryExitLogRequest request);
        Task<EntryExitLog> UpdateEntryExitLogAsync(UpdateEntryExitLogRequest request);
        Task<bool> DeleteEntryExitLogAsync(int id);
        Task<List<EntryExitLog>> Search(SearchEntryExitLogRequest request);
    }
}
