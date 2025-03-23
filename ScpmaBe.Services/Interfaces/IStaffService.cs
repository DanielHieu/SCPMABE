using ScpmaBe.Repositories.Entities;
using ScpmaBe.Services.Models;

namespace ScpmaBe.Services.Interfaces
{
    public interface IStaffService
    { 
        Task<List<Staff>> GetPaging(int pageIndex, int pageSize);

        Task<List<Staff>> GetStaffsOfOwnerAsync(int ownerId);

        Task<Staff> GetById(int id);

        Task<List<Staff>> SearchStaffAsync(SearchStaffRequest request);

        Task<Staff> RegisterStaffAsync(RegisterStaffRequest request);

        Task<Staff> AuthorizeAsync(string username, string password);

        Task<Staff> UpdateStaffAsync(UpdateStaffRequest Staff);

        Task<bool> DeleteStaffAsync(int id);
    }
}
                    