using ScpmaBe.Repositories.Entities;
using ScpmBe.Repositories.Infrastructure;

namespace ScpmaBe.Repositories.Interfaces
{
    public interface ITaskEachRepository : IRepository<TaskEach>
    {
        Task<bool> TaskEachIdExsistAsync(int id);
    }
}
