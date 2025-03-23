using Microsoft.AspNetCore.Mvc;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models;

namespace ScpmaBe.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FloorController : Controller
    {
        private readonly IFloorService _floorService;

        public FloorController(IFloorService areaService)
        {
            _floorService = areaService;
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _floorService.GetById(id);

            return Ok(entity);
        }

        [HttpGet("GetFloorsByArea")]
        public async Task<IActionResult> GetFloorsByArea(int areaId)
        {
            var list = await _floorService.GetFloorsByArea(areaId);

            return Ok(list);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddFloor([FromBody] AddFloorRequest request)
        {
            var entity = await _floorService.AddFloorAsync(request);

            return Ok(entity);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateFloor(UpdateFloorRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = await _floorService.UpdateFloorAsync(request);

            return Ok(entity);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFloor(int id)
        {
            var result = await _floorService.DeleteFloorAsync(id);

            return Ok(result);
        }
    }
}

