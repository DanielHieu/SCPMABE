using ScpmaBe.Repositories.Entities;
using ScpmaBe.Services.Models;

namespace ScpmaBe.Services.Interfaces
{
    public interface ITaskEachService 
    {
        Task<List<TaskEach>> GetPaging(int pageIndex, int pageSize);

        Task<List<TaskEach>> GetTaskEachOfOwnerAsync(int ownerId);

        Task<TaskEach> GetById(int id);

        Task<List<TaskEach>> SearchTaskEachAsync(SearchTaskRequest request);

        Task<TaskEach> AddTaskEachAsync(AddTaskEachRequest request);

        Task<TaskEach> UpdateTaskEachAsync(UpdateTaskEachRequest request);

        Task<bool> DeleteTaskEachAsync(int id);
    }
}
