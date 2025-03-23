using Microsoft.AspNetCore.Mvc;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models;

namespace ScpmaBe.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskEachController : ControllerBase
    {
        private readonly ITaskEachService _taskEachService;
        public TaskEachController(ITaskEachService taskEachService)
        {
            _taskEachService = taskEachService;
        }

        [HttpGet("GetTaskEach")]
        public async Task<IActionResult> GetTaskEachOfOwner(int ownerId)
        {
            var getTaskEach = _taskEachService.GetTaskEachOfOwnerAsync(ownerId);
            return Ok(await getTaskEach);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            var getById = await _taskEachService.GetById(id);
            return Ok(getById);
        }

        [HttpPost("SearchTaskEach")]
        public async Task<IActionResult> SearchTaskEach(SearchTaskRequest request)
        {
            var searchTaskEach = await _taskEachService.SearchTaskEachAsync(request);

            return Ok(searchTaskEach);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddTaskEach(AddTaskEachRequest request)
        {
            var task = await _taskEachService.AddTaskEachAsync(request);

            return Ok(task);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateTaskEach(UpdateTaskEachRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var task = await _taskEachService.UpdateTaskEachAsync(request);
            return Ok(task);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskEach(int id)
        {
            var result = await _taskEachService.DeleteTaskEachAsync(id);

            return Ok();
        }
    }
}
