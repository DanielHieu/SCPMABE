using ScpmaBe.Repositories.Entities;
using ScpmaBe.Services.Models;

namespace ScpmaBe.Services.Interfaces
{
    public interface IFeedbackService
    {

        Task<List<FeedbackResponse>> GetFeedbacksOfCustomerAsync(int customerId);

        Task<SearchFeedbackResponse> SearchFeedbacks(SearchFeedbackRequest request);

        Task<FeedbackResponse> GetById(int id);

        Task<FeedbackResponse> AddFeedbackAsync(AddFeedbackRequest request);

        Task<FeedbackResponse> UpdateFeedbackAsync(UpdateFeedbackRequest request);

        Task<bool> DeleteFeedbackAsync(int id);
        Task<bool> MarkAsReadAsync(int id);
        Task<int> CountFeedbacksAsync(string status);
        Task<bool> ReplyAsync(int id, string content);
    }
}
