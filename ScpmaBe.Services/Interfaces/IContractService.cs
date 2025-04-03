using ScpmaBe.Repositories.Entities;
using ScpmaBe.Services.Models;

namespace ScpmaBe.Services.Interfaces
{
    public interface IContractService 
    {
        Task<List<Contract>> GetPaging(int pageIndex, int pageSize);

        Task<Contract> GetByIdAsync(int id);

        Task<Contract> AddContractAsync(AddContractRequest request);

        Task<Contract> UpdateContractAsync(UpdateContractRequest request);

        Task<bool> DeleteContractAsync(int id);

        Task<List<ContractResponse>> GetContractsOfCustomerAsync(int customerId);
        Task<List<ContractResponse>> SearchContractAsync(SearchContractRequest request);

        Task<bool> Approve(int contractId);
        Task<bool> Pay(int contractId);
        Task<bool> Complete(int contractId);
    }
}
