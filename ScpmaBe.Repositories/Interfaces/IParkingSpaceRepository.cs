using ScpmaBe.Repositories.Entities;
using ScpmBe.Repositories.Infrastructure;

namespace ScpmaBe.Repositories.Interfaces
{
    public interface IParkingSpaceRepository : IRepository<ParkingSpace>
    {
        Task<bool> ParkingSpaceIdExsistAsync(int id);
    }
}
