using Adis.Bll.Dtos.Task;
using Adis.Bll.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Adis.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
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

        [HttpPost]
        public async Task<IActionResult> AddTask(PostTaskDto taskDto)
        {
            return Ok(await _taskService.AddTaskAsync(taskDto));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTask(PutTaskDto taskDto)
        {
            return Ok(await _taskService.UpdateTaskAsync(taskDto));
        }
    }
}
