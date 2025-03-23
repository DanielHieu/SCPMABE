using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmBe.Repositories.Infrastructure;

namespace ScpmaBe.Repositories
{
    public class PaymentContractRepository : Repository<PaymentContract>, IPaymentContractRepository
    {
        public PaymentContractRepository(SCPMContext dbContext) : base(dbContext) { }

        public override async Task<PaymentContract> GetById(int id)
        {
            return await Table.FirstOrDefaultAsync(x => x.PaymentContractId == id);
        }
    }
}
