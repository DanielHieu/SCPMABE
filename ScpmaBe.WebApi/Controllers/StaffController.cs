using Microsoft.AspNetCore.Mvc;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models;

namespace ScpmaBe.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;
        private readonly IParkingLotService _parkingLotService;
        public StaffController(
            IStaffService staffService, IParkingLotService parkingLotService)
        {
            _staffService = staffService;
            _parkingLotService = parkingLotService;
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            var getById = await _staffService.GetById(id);
            return Ok(getById);
        }

        [HttpPost("SearchStaff")]
        public async Task<IActionResult> SearchStaff([FromQuery] SearchStaffRequest request)
        {
            var searchStaff = await _staffService.SearchStaffAsync(request);

            return Ok(searchStaff);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAccount([FromBody] RegisterStaffRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var account = await _staffService.RegisterStaffAsync(request);

            return Ok(new
            {
                account.StaffId,
                account.OwnerId,
                account.FirstName,
                account.LastName,
                account.Phone,
                account.Username,
                account.Email,
                account.IsActive
            });
        }

        [HttpPost("authorize")]
        public async Task<IActionResult> Authorize([FromBody] StaffLoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var acc = await _staffService.AuthorizeAsync(request.Username, request.Password);
            var parkingLot = await _parkingLotService.GetById(request.ParkingLotId);

            return Ok(new
            {
                Success = true,
                User = new
                {
                    Id = acc.StaffId,
                    acc.Email,
                    acc.FirstName,
                    acc.LastName,
                    acc.Phone,
                    acc.Username
                },
                ParkingLot = parkingLot
            });
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateAccount(UpdateStaffRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var acc = await _staffService.UpdateStaffAsync(request);
            return Ok(acc);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var result = await _staffService.DeleteStaffAsync(id);

            return Ok();
        }
    }
}
