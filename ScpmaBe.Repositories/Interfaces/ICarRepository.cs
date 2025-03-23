using ScpmaBe.Repositories.Entities;
using ScpmBe.Repositories.Infrastructure;

namespace ScpmaBe.Repositories.Interfaces
{
    public interface ICarRepository : IRepository<Car>
    {
        Task<bool> CarIdExsistAsync(int id);
    }
}
