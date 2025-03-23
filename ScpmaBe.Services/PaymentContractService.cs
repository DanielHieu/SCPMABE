using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmaBe.Services.Interfaces;

namespace ScpmaBe.Services
{
    public class PaymentContractService : IPaymentContractService
    {
        private readonly IPaymentContractRepository _paymentContractRepository;
        private readonly IContractRepository _contractRepository;

        public PaymentContractService(IPaymentContractRepository paymentContractRepository, IContractRepository contractRepository)
        {
            _paymentContractRepository = paymentContractRepository;
            _contractRepository = contractRepository;
        }

        public async Task<List<PaymentContract>> GetPaging(int pageIndex, int pageSize)
        {
            return await _paymentContractRepository.GetAll().Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }
    }
}
