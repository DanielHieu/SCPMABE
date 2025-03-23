using ScpmaBe.Repositories.Entities;
using ScpmaBe.Services.Models;

namespace ScpmaBe.Services.Interfaces
{
    public interface IAssignedTaskService
    {
        Task<List<AssignedTask>> GetPaging(int pageIndex, int pageSize);

        //Task<List<AssignedTask>> GetAllAssignedTasksAsync();

        Task<List<AssignedTask>> GetAssignedTasksOfStaffAsync(int staffId);

        Task<AssignedTask> GetById(int id);

        Task<AssignedTask> AddAssignedTaskAsync(AddAssignedTaskRequest request);

        Task<AssignedTask> UpdateAssignedTaskAsync(UpdateAssignedTaskRequest request);

        Task<bool> DeleteAssignedTaskAsync(int id);
    }
}
