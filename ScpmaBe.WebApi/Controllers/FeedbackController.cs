using Microsoft.AspNetCore.Mvc;
using ScpmaBe.Services.Enum;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models;

namespace ScpmaBe.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;
        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpGet("GetFeedbacksOfCustomer")]
        public async Task<IActionResult> GetFeedbacksOfCustomer(int customerId)
        {
            var getFbOfCustomer = await _feedbackService.GetFeedbacksOfCustomerAsync(customerId);
            return Ok(getFbOfCustomer);
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search([FromBody] SearchFeedbackRequest request)
        {
            var result = await _feedbackService.SearchFeedbacks(new SearchFeedbackRequest
            {
                Keyword = request.Keyword,
                PageIndex = request.PageIndex <= 0 ? 1 : request.PageIndex,
                PageSize = request.PageSize <= 0 ? 10 : request.PageSize,
                Status = request.Status
            });

            return Ok(result);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            var getById = await _feedbackService.GetById(id);
            return Ok(getById);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddFeedback([FromBody] AddFeedbackRequest request)
        {
            var feedback = await _feedbackService.AddFeedbackAsync(request);
            return Ok(feedback);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateFeedback(UpdateFeedbackRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var fb = await _feedbackService.UpdateFeedbackAsync(request);

            return Ok(fb);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeedback(int id)
        {
            var result = await _feedbackService.DeleteFeedbackAsync(id);

            return Ok();
        }

        [HttpPut("{id}/MarkAsRead")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var result = await _feedbackService.MarkAsReadAsync(id);

            return Ok(new { success = result});
        }

        [HttpPost("{id}/Reply")]
        public async Task<IActionResult> Reply(int id, [FromBody]FeedbackResponseRequest request)
        {
            var result = await _feedbackService.ReplyAsync(id, request.Content);

            return Ok(new { success = result });
        }

        [HttpGet("Count")]
        public async Task<IActionResult> Count(string status)
        {
            var result = await _feedbackService.CountFeedbacksAsync(status);
            
            return Ok(result);
        }

        [HttpGet("CountStats")]
        public async Task<IActionResult> CountStats()
        {
            var status1 =  await _feedbackService.CountFeedbacksAsync(FeedbackStatus.New.ToString());
            var status2 = await _feedbackService.CountFeedbacksAsync(FeedbackStatus.Viewed.ToString());
            var status3 = await _feedbackService.CountFeedbacksAsync(FeedbackStatus.Responsed.ToString());

            return Ok(new
            {
                All = status1 + status2 + status3,
                New = status1,
                Viewed = status2,
                Responsed = status3
            });
        }
    }
}
