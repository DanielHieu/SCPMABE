using ScpmaBe.Repositories.Entities;
using ScpmBe.Repositories.Infrastructure;

namespace ScpmaBe.Repositories.Interfaces
{
    public interface IStaffRepository : IRepository<Staff>
    {
        Task<bool> StaffIdExsistAsync(int id);
    }
}
