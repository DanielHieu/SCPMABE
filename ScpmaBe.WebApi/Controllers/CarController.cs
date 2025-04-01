using Microsoft.AspNetCore.Mvc;
using ScpmaBe.Services.Common;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models;

namespace ScpmaBe.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly ICarService _carService;
        private readonly IImageService _imageService;
        private readonly AppSettings _appSettings;

        public CarController(ICarService carService, IImageService imageService, AppSettings appSettings)
        {
            _appSettings = appSettings;
            _carService = carService;
            _imageService = imageService;
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
            var result = await _carService.GetById(id);
            return Ok(result);
        }

        [HttpPost("SearchCar")]
        public async Task<IActionResult> SearchCar([FromBody] SearchCarRequest request)
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

        /// <summary>
        /// Max 5MB, Filetypes: ".jpg", ".jpeg", ".png", ".gif"
        /// </summary>
        /// <param name="imageFile"></param>
        /// <returns></returns>
        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadCarImage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return BadRequest("No file uploaded");

            // Validate file is an image
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
            string fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
                return BadRequest("Invalid file type. Only jpg, jpeg, png, and gif are allowed.");

            // Size validation (e.g., max 5MB)
            if (imageFile.Length > 5 * 1024 * 1024)
                return BadRequest("File size exceeds the limit of 5MB.");

            try
            {
                var fileName = Guid.NewGuid().ToString() + fileExtension;
                var folder = _carService.GetImageFolder();
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folder, fileName);
                var result = await _imageService.UploadImageAsync(filePath, imageFile.OpenReadStream());

                return Ok(result ? new
                {
                    PathFull = $"{_appSettings.ApplicationUrl}/{folder}/{fileName}",
                    FileName = fileName
                } : null);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
