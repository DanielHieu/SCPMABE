using ScpmaBe.Repositories.Entities;
using ScpmBe.Repositories.Infrastructure;

namespace ScpmaBe.Repositories.Interfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<bool> CustomerIdExsistAsync(int id);
    }
}
