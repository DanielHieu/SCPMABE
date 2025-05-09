using ScpmaBe.Repositories.Entities;
using ScpmaBe.Services.Models;

namespace ScpmaBe.Services.Interfaces
{
    public interface IStaffService
    {
        Task<Staff> AuthorizeAsync(string username, string password);

        Task<StaffResponse> GetById(int id);

        Task<List<StaffResponse>> SearchStaffAsync(SearchStaffRequest request);

        Task<Staff> AddStaffAsync(AddStaffRequest request);

        Task<Staff> UpdateStaffAsync(UpdateStaffRequest Staff);

        Task<bool> DeleteStaffAsync(int id);

        Task<bool> ResetPassword(int staffId);

        Task<List<StaffResponse>> GetAll();
        Task<List<TaskEachResponse>> GetTasks(int id);

        Task<bool> ChangePasswordAsync(StaffChangePasswordRequest request);
        Task<List<TaskEachResponse>> GetScheduleAsync(ScheduleRquest request);
    }
}
                    