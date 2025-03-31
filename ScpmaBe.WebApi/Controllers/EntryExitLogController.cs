using Microsoft.AspNetCore.Mvc;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models;

namespace ScpmaBe.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntryExitLogController : Controller
    {
        private readonly IEntryExitLogService _entryExitLogService;

        public EntryExitLogController(IEntryExitLogService entryExitLogService)
        {
            _entryExitLogService = entryExitLogService;
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _entryExitLogService.GetById(id);

            return Ok(entity);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddEntryExitLog([FromBody] AddEntryExitLogRequest request)
        {
            var entity = await _entryExitLogService.AddEntryExitLogAsync(request);

            return Ok(entity);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update(UpdateEntryExitLogRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = await _entryExitLogService.UpdateEntryExitLogAsync(request);

            return Ok(entity);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEntryExitLog(int id)
        {
            var result = await _entryExitLogService.DeleteEntryExitLogAsync(id);

            return Ok(result);
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(SearchEntryExitLogRequest request)
        {
            var result = await _entryExitLogService.Search(request);

            return Ok(result);
        }

        [HttpPost("CalculateFee")]
        public async Task<IActionResult> CalculateFee([FromBody] CalculateFeeRequest request)
        {
            var response = await _entryExitLogService.CalculateFeeAsync(request);

            return Ok(response);
        }

        [HttpGet("GetEntrancingCars/{parkingLotId}")]
        public async Task<IActionResult> GetEntrancingCars(int parkingLotId)
        {
            var result = await _entryExitLogService.GetEntrancingCars(parkingLotId);

            return Ok(result);
        }
    }
}

