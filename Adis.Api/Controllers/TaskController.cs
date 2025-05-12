using Adis.Bll.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Adis.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskDetailsById(int id)
        {
            var task = await _taskService.GetTaskDetailsByIdAsync(id);
            if(task == null)
                return NotFound("Задача с таким id не найдена");
            return Ok(task);
        }
    }
}
