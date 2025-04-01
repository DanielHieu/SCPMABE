using Microsoft.AspNetCore.Mvc;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models;

namespace ScpmaBe.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var result = await _customerService.DeleteCustomerAsync(id);

            return Ok();
        }
    }
}
