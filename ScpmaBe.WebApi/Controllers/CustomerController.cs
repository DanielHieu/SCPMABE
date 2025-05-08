using Microsoft.AspNetCore.Mvc;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models;

namespace ScpmaBe.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ICustomerService _customerService;
        
        public CustomerController(
            ILogger<CustomerController> logger,
            ICustomerService customerService)
        {
            _logger = logger;
            _customerService = customerService;
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            var getById = await _customerService.GetById(id);

            return Ok(getById);
        }

        [HttpPost("SearchCustomer")]
        public async Task<IActionResult> SearchCustomer([FromBody] SearchCustomerRequest request)
        {
            var searchCustomer = await _customerService.SearchCustomerAsync(request);

            return Ok(searchCustomer);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAccount([FromBody] RegisterCustomerRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var account = await _customerService.RegisterCustomerAsync(request);

            return Ok(new CustomerResponse
            {
                CustomerId = account.CustomerId,
                Email = account.Email,
                FirstName = account.FirstName,
                LastName = account.LastName,
                Phone = account.Phone,
                Username = account.Username
            });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAccount([FromBody] LoginCustomerRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var acc = await _customerService.AuthorizeAsync(request.Username, request.Password);

            return Ok(new
            {
                acc.CustomerId,
                acc.Username
            });
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateAccount(UpdateCustomerRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var acc = await _customerService.UpdateCustomerAsync(request);

            return Ok(acc);
        }

        [HttpPost("Approve")]
        public async Task<IActionResult> Approve(int customerId)
        {
            var result = await _customerService.ApproveCustomerAsync(customerId);
            return Ok(new { success = result });
        }

        [HttpPost("Disable")]
        public async Task<IActionResult> Disable([FromBody]AccountDisabledRequest request)
        {
            var result = await _customerService.DisableCustomerAsync(request.CustomerId, request.Reason);

            return Ok(new { success = result });
        }

        [HttpGet("{customerId}/chat")]
        public async Task<IActionResult> Chat(int customerId)
        {
            _logger.LogInformation("Chat page requested. Customer ID: {CustomerId}", customerId);

            try
            {
                var customer = await _customerService.GetById(customerId);

                if(customer == null)
                {
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "static", "chat.html");

                    string htmlContent = System.IO.File.ReadAllText(filePath);

                    return Content(htmlContent, "text/html");
                }
                else
                {
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "static", "chat_customer.html");

                    string htmlContent = System.IO.File.ReadAllText(filePath);

                    htmlContent = htmlContent.Replace("{{CustomerName}}", $"{customer.FirstName} {customer.LastName}")
                                             .Replace("{{CustomerEmail}}", customer.Email);

                    return Content(htmlContent, "text/html");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error serving chat page. Customer ID: {CustomerId}", customerId);

                // Fallback if file not found or other error
                return Content("<html><body><h1>Chat Page</h1><p>Unable to load the chat page at this time.</p></body></html>", "text/html");
            }
        }


        [HttpPost("ForgotPassword")] 
        public async Task<IActionResult> ForgotPassword([FromBody]ForgotPasswordRequest request)
        {
            var result = await _customerService.ForgotPasswordAsync(request.Email);

            return Ok(new { success = result });
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var result = await _customerService.ChangePasswordAsync(request);

            return Ok(new { success = result });
        }

        [HttpPost("Activate")]
        public async Task<IActionResult> Activate(ActivateRequest request)
        {
            var result = await _customerService.ActivateAccountAsync(request.Code);
            return Ok(new { success = result });
        }
    }
}
