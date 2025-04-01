using Microsoft.AspNetCore.Mvc;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models;

namespace ScpmaBe.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorController : ControllerBase
    {
        private readonly IParkingSpaceService _parkingSpaceService;
        private readonly IParkingStatusSensorService _parkingStatusSensorService;

        public SensorController(IParkingSpaceService parkingSpaceService, IParkingStatusSensorService parkingStatusSensorService)
        {
            _parkingSpaceService = parkingSpaceService;
            _parkingStatusSensorService = parkingStatusSensorService;
        }

        [HttpGet("ChangeStatus")]
        public async Task<bool> ChangeStatus()
        {
            if(Request.Headers.TryGetValue("ApiKey",out var value))
            {
                return await _parkingSpaceService.ChangeStatus(value.ToString());
            }

            return false;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _parkingStatusSensorService.GetAll();

            return Ok(result);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddSensor([FromBody]AddParkingStatusSensorRequest request)
        {
            var result = await _parkingStatusSensorService.AddSensor(request);

            return Ok(new { success = result});
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateSensor([FromBody]UpdateParkingStatusSensorRequest request)
        {
            var result = await _parkingStatusSensorService.UpdateSensor(request);

            return Ok(new { sucess = result});
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSensor(int id)
        {
            var result = await _parkingStatusSensorService.Delete(id);

            return Ok(new { sucess = result });
        }
    }
}
