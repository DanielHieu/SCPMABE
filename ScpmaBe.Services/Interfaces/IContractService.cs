using ScpmaBe.Repositories.Entities;
using ScpmaBe.Services.Models;

namespace ScpmaBe.Services.Interfaces
{
    public interface IContractService
    {
        Task<ContractResponse> GetByIdAsync(int id);

        Task<Contract> AddContractAsync(AddContractRequest request);

        Task<Contract> UpdateContractAsync(UpdateContractRequest request);

        Task<bool> DeleteContractAsync(int id);

        Task<List<ContractResponse>> SearchContractsAsync(SearchContractRequest request);

        Task<List<PaymentContractResponse>> GetPaymentContracts(int contractId);

        Task<bool> RejectPaymentContract(int paymentContractId, string reason);
        Task<bool> ApprovePaymentContract(int paymentContractId);
        Task<bool> PayPaymentContract(int paymentContractId);
        Task<bool> AcceptPaymentContract(int paymentContractId);

        Task<List<ContractResponse>> GetPendingContracts(GetContractsRequest request);
        Task<List<ContractResponse>> GetRejectedContracts(GetContractsRequest request);
        Task<List<ContractResponse>> GetApprovedContracts(GetContractsRequest request);
        Task<List<ContractResponse>> GetActivatedContracts(GetContractsRequest request);
        Task<List<ContractResponse>> GetPaidContracts(GetContractsRequest request);

        Task<bool> Renew(RenewRequest request);
        Task<bool> PayPaymentContract(int paymentContractId, string paymentMethod);
        Task<PaymentContract> GetPaymentContract(int paymentContractId);
        Task<List<ContractNeedToProcessResponse>> GetNeedToProcess();
        Task<List<ContractFutureExpiredResponse>> GetFutureExpired();

        Task<List<PaymentContractResponse>> GetPaymentHistories(int customerId);
    }
}
