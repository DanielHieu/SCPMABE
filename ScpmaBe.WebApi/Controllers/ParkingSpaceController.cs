using Microsoft.AspNetCore.Mvc;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models;

namespace ScpmaBe.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParkingSpaceController : ControllerBase
    {
        private readonly IParkingSpaceService _parkingSpaceService;

        public ParkingSpaceController(IParkingSpaceService ParkingSpaceService)
        {
            _parkingSpaceService = ParkingSpaceService;
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _parkingSpaceService.GetById(id);
            return Ok(entity);
        }

        [HttpGet("GetParkingSpacesByFloor")]
        public async Task<IActionResult> GetParkingSpacesByFloor(int floorId)
        {
            var list = await _parkingSpaceService.GetParkingSpacesByFloor(floorId);

            return Ok(list);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddParkingSpace([FromBody] AddParkingSpaceRequest request)
        {
            var entity = await _parkingSpaceService.AddParkingSpaceAsync(request);
            return Ok(entity);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateParkingSpace(UpdateParkingSpaceRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = await _parkingSpaceService.UpdateParkingSpaceAsync(request);

            return Ok(entity);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParkingSpace(int id)
        {
            var result = await _parkingSpaceService.DeleteParkingSpaceAsync(id);

            return Ok();
        }
    }
}