using Microsoft.AspNetCore.Mvc;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models;

namespace ScpmaBe.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignedTaskController : ControllerBase
    {
        private readonly IAssignedTaskService _assignedTaskService;

        public AssignedTaskController(IAssignedTaskService assignedTaskService)
        {
            _assignedTaskService = assignedTaskService;
        }

        [HttpGet("GetOfStaff")]
        public async Task<IActionResult> GetAssignedTaskOfStaff(int staffId)
        {
            var getTaskOfStaff = await _assignedTaskService.GetAssignedTasksOfStaffAsync(staffId);
            return Ok(getTaskOfStaff);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            var getById = await _assignedTaskService.GetById(id);
            return Ok(getById);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddAssignedTask(AddAssignedTaskRequest request)
        {
            var assTask = await _assignedTaskService.AddAssignedTaskAsync(request);
            return Ok(assTask);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateAssignedTask(UpdateAssignedTaskRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var assTask = await _assignedTaskService.UpdateAssignedTaskAsync(request);
            return Ok(assTask);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssignedTask(int id)
        {
            var result = await _assignedTaskService.DeleteAssignedTaskAsync(id);

            return Ok();
        }
    }
}
