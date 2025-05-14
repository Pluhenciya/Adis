using Adis.Bll.Dtos.Project;
using Adis.Bll.Dtos.Task;
using Adis.Bll.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Adis.Api.Controllers
{
    /// <summary>
    /// Позволяет управлять задачами
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController(ITaskService taskService) : ControllerBase
    {
        /// <inheritdoc cref="ITaskService"/>
        private readonly ITaskService _taskService = taskService;

        /// <summary>
        /// Возвращает подробности о задачу по идентификатору
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     GET /api/tasks/1
        ///
        /// </remarks>
        /// <param name="id">Идентификатор задаси</param>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="404">не найдена задача по идентификатору</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GetProjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
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
