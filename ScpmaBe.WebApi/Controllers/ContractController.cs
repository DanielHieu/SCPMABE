using Microsoft.AspNetCore.Mvc;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models;

namespace ScpmaBe.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private readonly IContractService _contractService;

        public ContractController(IContractService contractService)
        {
            _contractService = contractService;
        }

        [HttpGet("GetContractsOfOwner")]
        public async Task<IActionResult> GetContractsOfOwner(int ownerId)
        {
            var getParSpa = await _contractService.GetContractsOfOwnerAsync(ownerId);
            return Ok(getParSpa);
        }

        [HttpGet("GetContractsOfCustomer")]
        public async Task<IActionResult> GetContractsOfCustomer(int customerId)
        {
            var getParSpa = await _contractService.GetContractsOfCustomerAsync(customerId);
            return Ok(getParSpa);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            var getParSpa = await _contractService.GetByIdAsync(id);
            return Ok(getParSpa);
        }

        [HttpPost("SearchContract")]
        public async Task<IActionResult> SearchContract([FromQuery] SearchContractRequest request)
        {
            var searchContract = await _contractService.SearchContractAsync(request);
            return Ok(searchContract);
        }

        [HttpPost("AddContract")]
        public async Task<IActionResult> AddContract(AddContractRequest request)
        {
            var addContract = await _contractService.AddContractAsync(request);
            return Ok(addContract);
        }

        [HttpPut("UpdateContract")]
        public async Task<IActionResult> UpdateContract(UpdateContractRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updateContract = await _contractService.UpdateContractAsync(request); 
            return Ok(updateContract);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContract(int id)
        {
            var deleteContract = await _contractService.DeleteContractAsync(id);
            return Ok();
        }
    }
}
