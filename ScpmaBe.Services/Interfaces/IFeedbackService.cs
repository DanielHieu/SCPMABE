using ScpmaBe.Repositories.Entities;
using ScpmaBe.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScpmaBe.Services.Interfaces
{
    public interface IFeedbackService
    {
        Task<List<Feedback>> GetPaging(int pageIndex, int pageSize);

        Task<List<Feedback>> GetFeedbacksOfCustomerAsync(int customerId);

        Task<List<Feedback>> GetFeedbacksOfOwnerAsync(int ownerId);

        Task<Feedback> GetById(int id);

        Task<Feedback> AddFeedbackAsync(AddFeedbackRequest request);

        Task<Feedback> UpdateFeedbackAsync(UpdateFeedbackRequest request);

        Task<bool> DeleteFeedbackAsync(int id);
    }
}
