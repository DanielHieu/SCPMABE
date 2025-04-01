using Microsoft.AspNetCore.Mvc;
using ScpmaBe.Services.Enums;
using ScpmaBe.Services.Helpers;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models;

namespace ScpmaBe.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private readonly IContractService _contractService;
        private readonly IParkingLotService _parkingLotService;

        public ContractController(IContractService contractService, IParkingLotService parkingLotService)
        {
            _contractService = contractService;
            _parkingLotService = parkingLotService;
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _contractService.GetByIdAsync(id);

            return Ok(result);
        }

        [HttpGet("{id}/PaymentContracts")]
        public async Task<IActionResult> GetPaymentContracts(int id)
        {
            var result = await _contractService.GetPaymentContracts(id);

            return Ok(result);
        }

        [HttpPost("SearchContracts")]
        public async Task<IActionResult> SearchContracts([FromBody] SearchContractRequest request)
        {
            var searchContract = await _contractService.SearchContractsAsync(request);

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

            var entranceExisted = await _parkingLotService.CheckEntranceExisted(request.LicensePlate);

            if(entranceExisted)
            {
                return Ok(new
                {
                    Success = false,
                    Message = "Biển số xe đã tồn tại trong bãi đỗ xe."
                });
            }

            var items = await _contractService.SearchContractsAsync(new SearchContractRequest
            {
                LicensePlate = request.LicensePlate,
                ParkingLotId = request.ParkingLotId,
                Status = (int)ContractStatus.Active
            });

            var now = DateTime.Now.ToVNTime();

            var contract =
                items.Count == 0 ? null :
                items.FirstOrDefault(x => x.StartDate <= now && x.EndDate >= now);

            return Ok(new
            {
                Success = true,
                Contract = contract,
                Message = ""
            });
        }

        [HttpPost("Reject/{paymentContractId}")]
        public async Task<IActionResult> Reject(int paymentContractId, [FromBody] RejectContractRequest request)
        {
            var result = await _contractService.RejectPaymentContract(paymentContractId, request.Reason);

            return Ok(result);
        }

        [HttpPost("Approve/{paymentContractId}")]
        public async Task<IActionResult> Approve(int paymentContractId)
        {
            var result = await _contractService.ApprovePaymentContract(paymentContractId);

            return Ok(result);
        }

        [HttpPost("Pay/{paymentContractId}")]
        public async Task<IActionResult> Pay(int paymentContractId)
        {
            var result = await _contractService.PayPaymentContract(paymentContractId);
            return Ok(result);
        }

        [HttpPost("Accept/{paymentContractId}")]
        public async Task<IActionResult> Accept(int paymentContractId)
        {
            var result = await _contractService.AcceptPaymentContract(paymentContractId);
            return Ok(result);
        }

        [HttpGet("GetPendingContracts")]
        public async Task<IActionResult> GetPendingContracts(int customerId)
        {
            var result = await _contractService.GetPendingContracts(new GetContractsRequest
            {
                CustomerId = customerId
            });

            return Ok(result);
        }

        [HttpGet("GetRejectedContracts")]
        public async Task<IActionResult> GetRejectedContracts(int customerId)
        {
            var result = await _contractService.GetRejectedContracts(new GetContractsRequest
            {
                CustomerId = customerId
            });

            return Ok(result);
        }

        [HttpGet("GetApprovedContracts")]
        public async Task<IActionResult> GetApprovedContracts(int customerId)
        {
            var result = await _contractService.GetApprovedContracts(new GetContractsRequest
            {
                CustomerId = customerId
            });

            return Ok(result);
        }

        [HttpGet("GetPaidContracts")]
        public async Task<IActionResult> GetPaidContracts(int customerId)
        {
            var result = await _contractService.GetPaidContracts(new GetContractsRequest
            {
                CustomerId = customerId
            });

            return Ok(result);
        }

        [HttpGet("GetActivatedContracts")]
        public async Task<IActionResult> GetActivatedContracts(int customerId)
        {
            var result = await _contractService.GetActivatedContracts(new GetContractsRequest
            {
                CustomerId = customerId
            });

            return Ok(result);
        }

        [HttpPost("Renew")]
        public async Task<IActionResult> Renew(RenewRequest request)
        {
            var result = await _contractService.Renew(request);
            return Ok(result);
        }

        [HttpGet("NeedToProcess")]
        public async Task<IActionResult> NeedToProcess()
        {
            var result = await _contractService.GetNeedToProcess();

            return Ok(result);
        }

        [HttpGet("FutureExpired")]
        public async Task<IActionResult> GetFutureExpired()
        {
            var result = await _contractService.GetFutureExpired();

            return Ok(result);
        }
    }
}
