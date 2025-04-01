using Microsoft.AspNetCore.Mvc;
using ScpmaBe.Services.Helpers;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models;
using ScpmaBe.WebApi.Helpers;

namespace ScpmaBe.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : ControllerBase
    {
        private readonly IOwnerService _ownerService;
        public OwnerController(IOwnerService ownerService)
        {
            _ownerService = ownerService;
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _ownerService.GetById(id);

            return Ok(entity);
        }

        [HttpPost("Authorize")]
        public async Task<IActionResult> Authorize([FromBody] LoginOwnerRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var acc = await _ownerService.AuthorizeAsync(request.Username, request.Password);

            return Ok(new
            {
                Id = acc?.OwnerId,
                acc?.Username,
                acc?.Email,
                Role = "owner",
                AccessToken = AccessTokenGenerator.GenerateExpiringAccessToken(DateTime.Now.ToVNTime().AddDays(1)),
            });
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateAccount([FromBody]UpdateOwnerRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var acc = await _ownerService.UpdateOwnerAsync(request);

            return Ok(acc);
        }
    }
}
