using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("GetFeedbacksOfOwner")]
        public async Task<IActionResult> GetFeedbacksOfOwner(int ownerId)
        {
            var getFbOfOwner = await _feedbackService.GetFeedbacksOfOwnerAsync(ownerId);
            return Ok(getFbOfOwner);
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
    }
}
