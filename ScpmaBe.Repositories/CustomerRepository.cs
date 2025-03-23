using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmBe.Repositories.Infrastructure;

namespace ScpmaBe.Repositories.Repo
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(SCPMContext _context) : base(_context) { }

        public override async Task<Customer> GetById(int id)
        {
            return await Table.FirstOrDefaultAsync(x => x.CustomerId == id);
        }

        public async Task<bool> CustomerIdExsistAsync(int id)
        {
            return await Table.AnyAsync(y => y.CustomerId == id);
        }
    }
}
