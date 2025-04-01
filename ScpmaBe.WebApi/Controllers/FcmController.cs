using Microsoft.AspNetCore.Mvc;
using ScpmaBe.Services.Models;

namespace ScpmaBe.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FcmController : Controller
    {
        private readonly ILogger<FcmController> _logger;

        public FcmController(ILogger<FcmController> logger)
        {
            _logger = logger;
        }   

        [HttpPost("RegisterDevice")]
        public async Task<IActionResult> RegisterDevice([FromBody] RegisterDeviceRequest request)
        {
            _logger.LogInformation("Registering device with token: {CustomerId} - {Token} - {DeviceId}", request.CustomerId, request.DeviceToken, request.DeviceId);

            // Simulate device registration logic
            await Task.Delay(100); // Simulate async work

            return Ok(new { Message = "Device registered successfully." });
        }
    }
}
