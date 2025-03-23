using Microsoft.AspNetCore.Mvc;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models;

namespace ScpmaBe.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AreaController : Controller
    {
        private readonly IAreaService _areaService;

        public AreaController(IAreaService areaService)
        {
            _areaService = areaService;
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _areaService.GetById(id);

            return Ok(entity);
        }


        [HttpGet("GetAreasByParkingLot")]
        public async Task<IActionResult> GetAreasByParkingLot(int parkingLotId)
        {
            var list = await _areaService.GetAreasByParkingLot(parkingLotId);

            return Ok(list);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddArea([FromBody] AddAreaRequest request)
        {
            var entity = await _areaService.AddAreaAsync(request);

            return Ok(entity);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateArea(UpdateAreaRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = await _areaService.UpdateAreaAsync(request);

            return Ok(entity);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArea(int id)
        {
            var result = await _areaService.DeleteAreaAsync(id);

            return Ok();
        }
    }
}

