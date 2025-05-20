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
            IStaffService staffService, 
            IParkingLotService parkingLotService)
        {
            _staffService = staffService;
            _parkingLotService = parkingLotService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] StaffLoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var acc = await _staffService.AuthorizeAsync(request.Username, request.Password);

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
                    acc.Username,
                    Name = $"{acc.FirstName} {acc.LastName}"
                }
            });
        }

        [HttpPost("Authorize")]
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
                ParkingLot = parkingLot != null ? new ParkingLotResponse
                {
                    ParkingLotId = parkingLot.ParkingLotId,
                    Address = parkingLot.Address,
                    Name = parkingLot.ParkingLotName,
                    ParkingLotName = parkingLot.ParkingLotName
                } : null
            });
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            var getById = await _staffService.GetById(id);
            return Ok(getById);
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search([FromBody] SearchStaffRequest request)
        {
            var searchStaff = await _staffService.SearchStaffAsync(request);

            return Ok(searchStaff);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddAccount([FromBody]AddStaffRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var account = await _staffService.AddStaffAsync(request);

            return Ok(new
            {
                account.StaffId,
                account.FirstName,
                account.LastName,
                account.Phone,
                account.Username,
                account.Email,
                account.IsActive
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

        [HttpPost("ResetPassword/{staffId}")]
        public async Task<IActionResult> ResetPassword(int staffId)
        {
            var result = await _staffService.ResetPassword(staffId);
            return Ok(new { success = result});
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _staffService.GetAll();
            return Ok(result);
        }

        [HttpGet("{id}/tasks")]
        public async Task<IActionResult> GetTasks(int id)
        {
            var result = await _staffService.GetTasks(id);
            return Ok(result);
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody]StaffChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _staffService.ChangePasswordAsync(request);

            return Ok(new { success = result });
        }

        [HttpPost("Schedule")]
        public async Task<IActionResult> GetSchedule([FromBody]ScheduleRquest request)
        {

            var result = await _staffService.GetScheduleAsync(request);

            return Ok(result);
        }
    }
}
