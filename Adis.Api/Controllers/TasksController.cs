using Adis.Bll.Dtos.Project;
using Adis.Bll.Dtos.Task;
using Adis.Bll.Interfaces;
using Adis.Dm;
using Microsoft.AspNetCore.Authorization;
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
            if (task == null)
                return NotFound("Задача с таким id не найдена");
            return Ok(task);
        }

        /// <summary>
        /// Добавляет новую задачу
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     POST /api/tasks
        ///     {
        ///         "name": "Разработка сметы проекта",
        ///         "description": "Смета должна соответствовать гостам и выполнена в Гранд-Смете",
        ///         "idPerformers": [45, 78],
        ///         "idCheckers": [32],
        ///         "endDate": "2025-11-30",
        ///         "idProject": 15,
        ///         "status": "ToDo"
        ///     }
        ///
        /// </remarks>
        /// <param name="taskDto">Данные новой задачи</param>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="400">Ошибка валидации данных</response>
        /// <response code="401">Неавторизованный пользователь</response>
        /// <response code="403">Пользователь без прав на это действие</response>
        [HttpPost]
        [ProducesResponseType(typeof(TaskDetailsDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> AddTask(PostTaskDto taskDto)
        {
            return Ok(await _taskService.AddTaskAsync(taskDto));
        }

        /// <summary>
        /// Изменяет данные задачи
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     PUT /api/tasks
        ///     {
        ///         "name": "Разработка сметы проекта",
        ///         "description": "Смета должна соответствовать гостам и выполнена в Гранд-Смете",
        ///         "idPerformers": [45, 78],
        ///         "idCheckers": [32],
        ///         "endDate": "2025-11-30",
        ///         "idProject": 15,
        ///         "status": "ToDo"
        ///     }
        ///
        /// </remarks>
        /// <param name="taskDto">Новые данные задачи</param>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="400">Ошибка валидации данных</response>
        /// <response code="401">Неавторизованный пользователь</response>
        /// <response code="403">Пользователь без прав на это действие</response>
        [HttpPut]
        [ProducesResponseType(typeof(TaskDetailsDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> UpdateTask(PutTaskDto taskDto)
        {
            return Ok(await _taskService.UpdateTaskAsync(taskDto));
        }

        /// <summary>
        /// Возвращает задачи для пользователя
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     GET /api/tasks
        ///
        /// </remarks>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="401">Неавторизованный пользователь</response>
        /// <response code="403">Пользователь без прав на это действие</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TaskDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [Authorize(Roles = "Projecter")]
        public async Task<IActionResult> GetTasksForProjecter()
        {
            return Ok(await _taskService.GetTaskForProjecterAsync());
        }

        [HttpGet("{id}/{status}")]
        [Authorize(Roles = "Projecter, Admin")]
        public async Task<IActionResult> UpdateTaskStatus(int id, string status)
        {
            try
            {
                return Ok(await _taskService.UpdateTaskStatusAsync(id, status));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{idTask}")]
        [Authorize(Roles = "Projecter, Admin")]
        public async Task<IActionResult> UpdateTaskResult(int idTask, [FromBody] TaskResultDto dto)
        {
            try
            {
                return Ok(await _taskService.UpdateTaskResultAsync(idTask, dto.Result));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
