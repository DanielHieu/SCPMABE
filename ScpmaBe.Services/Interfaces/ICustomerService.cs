using ScpmaBe.Repositories.Entities;
using ScpmaBe.Services.Models;

namespace ScpmaBe.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<List<Customer>> GetCustomersOfOwnerAsync(int ownerId);

        Task<Customer> GetById(int id);

        Task<List<CustomerResponse>> SearchCustomerAsync(SearchCustomerRequest request);

        Task<Customer> RegisterCustomerAsync(RegisterCustomerRequest request);

        Task<Customer> AuthorizeAsync(string username, string password);

        Task<Customer> UpdateCustomerAsync(UpdateCustomerRequest request);

        Task<bool> DeleteCustomerAsync(int id);
        Task<bool> ApproveCustomerAsync(int customerId);
        Task<bool> DisableCustomerAsync(int customerId, string reason);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ChangePasswordAsync(ChangePasswordRequest request);
        Task<bool> ActivateAccountAsync(string code);
    }
}
