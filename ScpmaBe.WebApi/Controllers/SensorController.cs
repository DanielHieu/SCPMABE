﻿using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> AddSensor([FromBody]AddParkingStausSensorRequest request)
        {
            var result = await _parkingStatusSensorService.AddSensor(request);

            return Ok(result);
        }

        [HttpPost("Update")]
        public async Task<IActionResult> UpdateSensor([FromBody]UpdateParkingStatusSensorRequest request)
        {
            var result = await _parkingStatusSensorService.UpdateSensor(request);

            return Ok(result);
        }
    }
}
