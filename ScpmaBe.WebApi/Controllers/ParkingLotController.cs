using Microsoft.AspNetCore.Mvc;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models;

namespace ScpmaBe.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParkingLotController : Controller
    {
        private readonly IParkingLotService _parkingLotService;
        public ParkingLotController(IParkingLotService ParkingLotService)
        {
            _parkingLotService = ParkingLotService;
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _parkingLotService.GetById(id);
            return Ok(new
            {
                entity.ParkingLotId,
                Name = entity.ParkingLotName,
                entity.ParkingLotName,
                entity.Address,
                entity.Lat,
                entity.Long,
                entity.PricePerDay,
                entity.PricePerMonth,
                entity.PricePerHour
            });
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _parkingLotService.Search(new SearchParkingLotRequest
            {
                Keyword = ""
            });

            return Ok(result);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddParkingLot([FromBody] AddParkingLotRequest request)
        {
            var entity = await _parkingLotService.AddParkingLotAsync(request);

            return Ok(new
            {
                entity.ParkingLotId,
                Name = entity.ParkingLotName,
                entity.ParkingLotName,
                entity.Address,
                entity.Lat,
                entity.Long,
                entity.PricePerDay,
                entity.PricePerMonth,
                entity.PricePerHour
            });
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateParkingLot(UpdateParkingLotRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = await _parkingLotService.UpdateParkingLotAsync(request);

            return Ok(new
            {
                entity.ParkingLotId,
                Name = entity.ParkingLotName,
                entity.ParkingLotName,
                entity.Address,
                entity.Lat,
                entity.Long,
                entity.PricePerDay,
                entity.PricePerMonth,
                entity.PricePerHour
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParkingLot(int id)
        {
            var result = await _parkingLotService.DeleteParkingLotAsync(id);

            return Ok();
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(SearchParkingLotRequest request)
        {
            var result = await _parkingLotService.Search(request);

            return Ok(result);
        }

        [HttpGet("{id}/Full")]
        public async Task<IActionResult> GetFull(int id)
        {
            var result = await _parkingLotService.GetFull(id);

            return Ok(result);
        }

        [HttpGet("{id}/SummaryStatuses")]
        public async Task<IActionResult> GetSummaryStatuses(int id)
        {
            var result = await _parkingLotService.GetSummaryStatuses(id);

            return Ok(result);
        }

        [HttpGet("{id}/Summaries")]
        public async Task<IActionResult> GetSummaries(int id)
        {
            var result = await _parkingLotService.GetSummaries(id);

            return Ok(result);
        }

        [HttpPost("SearchAvailablesForContract")]
        public async Task<IActionResult> SearchAvailablesForContract([FromBody] SearchAvailablesForContractRequest request)
        {
            var result = await _parkingLotService.GetAvailablesForContract(request);

            return Ok(result);
        }

        [HttpGet("{id}/Stats")]
        public async Task<IActionResult> Stats(int id)
        {
            var result = await _parkingLotService.GetStats(id);
            return Ok(result);
        }
    }
}

