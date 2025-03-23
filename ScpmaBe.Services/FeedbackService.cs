using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models;
using ScpmBe.Services.Exceptions;

namespace ScpmaBe.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedBackRepository _feedbackRepo;
        private readonly ICustomerRepository _customerRepo;

        public FeedbackService(IFeedBackRepository feedbackRepo, ICustomerRepository customerRepo)
        {
            _feedbackRepo = feedbackRepo;
            _customerRepo = customerRepo;
        }

        public async Task<List<Feedback>> GetPaging(int pageIndex, int pageSize)
        {
            return await _feedbackRepo.GetAll().Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<List<Feedback>> GetFeedbacksOfCustomerAsync(int customerId)
        {
            return await _feedbackRepo.GetAll()
                                      .Where(x => x.CustomerId == customerId)
                                      .ToListAsync();
        }

        public async Task<List<Feedback>> GetFeedbacksOfOwnerAsync(int ownerId)
        {
            return await _feedbackRepo.GetAll()
                                      .Where(y => y.Customer.OwnerId == ownerId)
                                      .ToListAsync();
        }

        public async Task<Feedback> GetById(int id)
        {
            var fb = await _feedbackRepo.GetById(id);

            if (fb == null) throw AppExceptions.NotFoundFeedBackId();

            return fb;
        }

        public async Task<Feedback> AddFeedbackAsync(AddFeedbackRequest request)
        {
            var newFeedBack = new Feedback
            {
                CustomerId = request.CustomerId,
                Message = request.Message,
                DateSubmitted = DateTime.Now
            };

            return await _feedbackRepo.Insert(newFeedBack);
        }

        public async Task<Feedback> UpdateFeedbackAsync(UpdateFeedbackRequest request)
        {
            var updateFb = await _feedbackRepo.GetById(request.FeedbackId);

            if (updateFb == null) throw AppExceptions.NotFoundFeedBackId();

            updateFb.Message = request.Message;
            updateFb.DateSubmitted = DateTime.Now;

            await _feedbackRepo.Update(updateFb);

            return updateFb;
        }

        public async Task<bool> DeleteFeedbackAsync(int id)
        {
            try
            {
                var fb = await _feedbackRepo.GetById(id);

                if (fb == null) throw AppExceptions.NotFoundFeedBackId();

                await _feedbackRepo.Delete(id);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
