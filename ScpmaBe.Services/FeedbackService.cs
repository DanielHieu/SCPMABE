using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmaBe.Services.Enum;
using ScpmaBe.Services.Helpers;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models;
using ScpmBe.Services.Exceptions;

namespace ScpmaBe.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedBackRepository _feedbackRepo;

        public FeedbackService(IFeedBackRepository feedbackRepo, ICustomerRepository customerRepo)
        {
            _feedbackRepo = feedbackRepo;
        }

        public async Task<List<Feedback>> GetPaging(int pageIndex, int pageSize)
        {
            return await _feedbackRepo.GetAll().Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<List<FeedbackResponse>> GetFeedbacksOfCustomerAsync(int customerId)
        {
            return await _feedbackRepo.GetAll()
                                      .Include(x=>x.Customer)
                                      .Where(x => x.CustomerId == customerId)
                                      .Select(fb => new FeedbackResponse
                                      {
                                          FeedbackId = fb.FeedbackId,
                                          Status = ((FeedbackStatus)fb.Status).ToString(),
                                          CustomerName = $"{fb.Customer.FirstName} {fb.Customer.LastName}",
                                          CustomerEmail = fb.Customer.Email,
                                          Content = fb.Message,
                                          CreatedAt = fb.DateSubmitted.ToString("dd/MM/yyyy HH:mm:ss"),
                                          ResponsedContent = fb.ResponsedContent,
                                          ResponsedAt = fb.ResponsedAt.HasValue ? fb.ResponsedAt.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
                                          CustomerId = fb.CustomerId
                                      })
                                      .ToListAsync();
        }

        public async Task<FeedbackResponse> GetById(int id)
        {
            var fb = await _feedbackRepo.GetAll().Include(x=>x.Customer).FirstOrDefaultAsync(x=> x.FeedbackId == id);

            if (fb == null) throw AppExceptions.NotFoundFeedBackId();

            return new FeedbackResponse
            {
                FeedbackId = fb.FeedbackId,
                Status = ((FeedbackStatus)fb.Status).ToString(),
                CustomerName = $"{fb.Customer.FirstName} {fb.Customer.LastName}",
                CustomerEmail = fb.Customer.Email,
                Content = fb.Message,
                CreatedAt = fb.DateSubmitted.ToString("dd/MM/yyyy HH:mm:ss"),
                ResponsedContent = fb.ResponsedContent,
                ResponsedAt = fb.ResponsedAt.HasValue ? fb.ResponsedAt.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
                CustomerId = fb.CustomerId
            };
        }

        public async Task<FeedbackResponse> AddFeedbackAsync(AddFeedbackRequest request)
        {
            var newFeedBack = new Feedback
            {
                CustomerId = request.CustomerId,
                Message = request.Message,
                DateSubmitted = DateTime.Now.ToVNTime(),
                Status = (int)FeedbackStatus.New,
                ResponsedContent = "",
                ResponsedAt = null
            };

            var feedback = await _feedbackRepo.Insert(newFeedBack);

            return new FeedbackResponse
            {
                FeedbackId = feedback.FeedbackId,
                Status = ((FeedbackStatus)feedback.Status).ToString(),
                CustomerName = "",
                CustomerEmail = "",
                Content = feedback.Message,
                CreatedAt = feedback.DateSubmitted.ToString("dd/MM/yyyy HH:mm:ss"),
                ResponsedContent = "",
            };
        }

        public async Task<FeedbackResponse> UpdateFeedbackAsync(UpdateFeedbackRequest request)
        {
            var updateFb = await _feedbackRepo.GetById(request.FeedbackId);

            if (updateFb == null) throw AppExceptions.NotFoundFeedBackId();

            if(updateFb.Status == (int)FeedbackStatus.Responsed)
                throw AppExceptions.FeedBackAlreadyReponsed();

            updateFb.Status = (int)FeedbackStatus.New;
            updateFb.Message = request.Message;

            await _feedbackRepo.Update(updateFb);

            return new FeedbackResponse
            {
                FeedbackId = updateFb.FeedbackId,
                Status = ((FeedbackStatus)updateFb.Status).ToString(),
                CustomerName = "",
                CustomerEmail = "",
                Content = updateFb.Message,
                CreatedAt = updateFb.DateSubmitted.ToString("dd/MM/yyyy HH:mm:ss"),
                ResponsedContent = updateFb.ResponsedContent,
            };
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

        public async Task<SearchFeedbackResponse> SearchFeedbacks(SearchFeedbackRequest request)
        {
            var total = await _feedbackRepo.GetAll().CountAsync();

            List<FeedbackResponse> items;
            
            if(total == 0)
                items = new List<FeedbackResponse>();
            else
            {
                var query = _feedbackRepo.GetAll().Include(x => x.Customer)
                               .OrderBy(x => x.Status).ThenByDescending(x => x.DateSubmitted)
                               .AsQueryable();

                if (!string.IsNullOrEmpty(request.Keyword))
                {
                     query = query.Where(x => 
                                         x.Customer.FirstName.Contains(request.Keyword) || 
                                         x.Customer.LastName.Contains(request.Keyword) || 
                                         x.Customer.Email.Contains(request.Keyword) || 
                                         x.Message.Contains(request.Keyword));
                }

                if (!string.IsNullOrEmpty(request.Status))
                {
                    if(request.Status == FeedbackStatus.Viewed.ToString())
                        query = query.Where(x => x.Status == (int)FeedbackStatus.Viewed);
                    else if (request.Status == FeedbackStatus.New.ToString())
                        query = query.Where(x => x.Status == (int)FeedbackStatus.New);
                    else if (request.Status == FeedbackStatus.Responsed.ToString())
                        query = query.Where(x => x.Status == (int)FeedbackStatus.Responsed);
                }

                items = await query.Skip((request.PageIndex - 1) * request.PageSize)
                               .Take(request.PageSize)
                               .Select(x => new FeedbackResponse
                               {
                                   FeedbackId = x.FeedbackId,
                                   CustomerId = x.CustomerId,
                                   Content = x.Message,
                                   CustomerName = $"{x.Customer.FirstName} {x.Customer.LastName}",
                                   CustomerEmail = x.Customer.Email,
                                   CreatedAt = x.DateSubmitted.ToString("dd/MM/yyyy HH:mm:ss"),
                                   Status = ((FeedbackStatus)x.Status).ToString(),
                                   ResponsedContent = x.ResponsedContent,
                                   ResponsedAt = x.ResponsedAt.HasValue ? x.ResponsedAt.Value.ToString("dd/MM/yyyy HH:mm:ss") : ""
                               })
                               .ToListAsync();
            }

            return new SearchFeedbackResponse
            {
                TotalCount = total,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = items
            };
        }

        public async Task<bool> MarkAsReadAsync(int id)
        {
            var fb = await _feedbackRepo.GetById(id);

            if (fb == null) throw AppExceptions.NotFoundFeedBackId();
            
            fb.Status = (int)FeedbackStatus.Viewed;
            
            await _feedbackRepo.Update(fb);
            
            return true;
        }

        public async Task<int> CountFeedbacksAsync(string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return await _feedbackRepo.GetAll().CountAsync();
            }

            if (status == "Viewed")
            {
                return await _feedbackRepo.GetAll().Where(x => x.Status == (int)FeedbackStatus.Viewed).CountAsync();
            }

            if (status == "New")
            {
                return await _feedbackRepo.GetAll().Where(x => x.Status == (int)FeedbackStatus.New).CountAsync();
            }

            if (status == "Responsed")
            {
                return await _feedbackRepo.GetAll().Where(x => x.Status == (int)FeedbackStatus.Responsed).CountAsync();
            }

            return await _feedbackRepo.GetAll().CountAsync();
        }

        public async Task<bool> ReplyAsync(int id, string content)
        {
            try
            {
                var feedBack = await _feedbackRepo.GetById(id);

                if (feedBack == null) throw AppExceptions.NotFoundFeedBackId();

                if (feedBack.Status == (int)FeedbackStatus.Responsed)
                    throw AppExceptions.FeedBackAlreadyReponsed();

                feedBack.Status = (int)FeedbackStatus.Responsed;
                feedBack.ResponsedContent = content;
                feedBack.ResponsedAt = DateTime.Now.ToVNTime();

                await _feedbackRepo.Update(feedBack);

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}
