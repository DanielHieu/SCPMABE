using Microsoft.AspNetCore.Mvc;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models;

namespace ScpmaBe.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly ICarService _carService;

        public CarController(ICarService carService)
        {
            _carService = carService;
        }

        [HttpGet("GetCarsOfCustomer")]
        public async Task<IActionResult> GetCarsOfCustomer(int customerId)
        {
            var getCarsOfCustomer = await _carService.GetCarsOfCustomerAsync(customerId);
            return Ok(getCarsOfCustomer);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            var getById = await _carService.GetById(id);
            return Ok(getById);
        }

        [HttpPost("SearchCar")]
        public async Task<IActionResult> SearchCar([FromQuery] SearchCarRequest request)
        {
            var search = await _carService.SearchCarAsync(request);
            return Ok(search);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddCar(AddCarRequest request)
        {
            var car = await _carService.AddCarAsync(request);
            return Ok(car);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateCar(UpdateCarRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var car = await _carService.UpdateCarAsync(request);
            return Ok(car);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var result = await _carService.DeleteCarAsync(id);

            return Ok();
        }
    }
}
