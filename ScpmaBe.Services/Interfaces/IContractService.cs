using ScpmaBe.Repositories.Entities;
using ScpmaBe.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScpmaBe.Services.Interfaces
{
    public interface IContractService 
    {
        Task<List<Contract>> GetPaging(int pageIndex, int pageSize);

        Task<List<Contract>> GetContractsOfCustomerAsync(int customerId);

        Task<List<Contract>> GetContractsOfOwnerAsync(int ownerId);

        Task<Contract> GetByIdAsync(int id);

        Task<List<Contract>> SearchContractAsync(SearchContractRequest request);

        Task<Contract> AddContractAsync(AddContractRequest request);

        Task<Contract> UpdateContractAsync(UpdateContractRequest request);

        Task<bool> DeleteContractAsync(int id);
    }
}
