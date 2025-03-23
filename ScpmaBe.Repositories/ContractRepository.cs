using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmBe.Repositories.Infrastructure;

namespace ScpmaBe.Repositories
{
    public class ContractRepository : Repository<Contract>, IContractRepository
    {
        public ContractRepository(SCPMContext _context) : base(_context) { }

        public override async Task<Contract> GetById(int id)
        {
            return await Table.FirstOrDefaultAsync(x => x.ContractId == id); 
        }
    }
}
