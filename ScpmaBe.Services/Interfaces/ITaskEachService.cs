using ScpmaBe.Services.Models;

namespace ScpmaBe.Services.Interfaces
{
    public interface ITaskEachService 
    {
        Task<TaskEachResponse> GetById(int id);

        Task<List<TaskEachResponse>> SearchTaskEachAsync(SearchTaskRequest request);

        Task<TaskEachResponse> AddTaskEachAsync(AddTaskEachRequest request);

        Task<TaskEachResponse> UpdateTaskEachAsync(UpdateTaskEachRequest request);

        Task<bool> DeleteTaskEachAsync(int id);
        Task<bool> CompleteAsync(int id);
    }
}
