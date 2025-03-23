using ScpmaBe.Repositories.Entities;
using ScpmaBe.Services.Models;

namespace ScpmaBe.Services.Interfaces
{
    public interface IOwnerService
    {
        Task<Owner> GetById(int id);

        Task<Owner> AuthorizeAsync(string username, string password);

        Task<Owner> UpdateOwnerAsync(UpdateOwnerRequest request);
    }
}
