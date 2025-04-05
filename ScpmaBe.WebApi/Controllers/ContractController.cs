using Microsoft.AspNetCore.Mvc;
using ScpmaBe.Services.Enums;
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

        [HttpPost("GetByLicensePlateAndParkingLot")]
        public async Task<IActionResult> GetByLicensePlateAndParkingLot([FromBody] CheckContractRequest request)
        {
            var items = await _contractService.SearchContractAsync(new SearchContractRequest
            {
                LicensePlate = request.LicensePlate,
                ParkingLotId = request.ParkingLotId,
                Status = (int)ContractStatus.Active
            });

            var contract =
                items.Count == 0 ? null :
                items.First();

            return Ok(new
            {
                Contract = contract
            });
        }

        [HttpPost("{contractId}/reject")]
        public async Task<IActionResult> Reject(int contractId)
        {
            var result = await _contractService.Reject(contractId);

            return Ok(result);
        }

        [HttpPost("{contractId}/approve")]
        public async Task<IActionResult> Approve(int contractId)
        {
            var result = await _contractService.Approve(contractId);

            return Ok(result);
        }

        [HttpPost("{contractId}/pay")]
        public async Task<IActionResult> Pay(int contractId)
        {
            var result = await _contractService.Pay(contractId);
            return Ok(result);
        }

        [HttpPost("{contractId}/complete")]
        public async Task<IActionResult> Complete(int contractId)
        {
            var result = await _contractService.Complete(contractId);
            return Ok(result);
        }

        [HttpGet("GetContractPendings")]
        public async Task<IActionResult> GetContractPendings()
        {
            return Ok(new object[] { });
        }

        [HttpGet("GetContractRejecteds")]
        public async Task<IActionResult> GetContractRejecteds()
        {
            return Ok(new object[] { });
        }

        [HttpGet("GetContractApproveds")]
        public async Task<IActionResult> GetContractApproveds()
        {
            return Ok(new object[] { });
        }

        [HttpGet("GetContractPaids")]
        public async Task<IActionResult> GetContractPaids()
        {
            return Ok(new object[] { });
        }

        [HttpGet("GetContractActivateds")]
        public async Task<IActionResult> GetContractActivateds()
        {
            return Ok(new object[] { });
        }
    }
}
