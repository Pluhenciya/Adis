using Adis.Bll.Dtos;
using Adis.Bll.Interfaces;
using Adis.Dm;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Adis.Api.Controllers
{
    /// <summary>
    /// Позволяет управлять проектами
    /// </summary>
    /// <param name="projectService"></param>
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController(IProjectService projectService) : ControllerBase
    {
        private readonly IProjectService _projectService = projectService;

        /// <summary>
        /// Добавляет новый проект
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     POST /api/projects
        ///     {
        ///         "name": "​Капитальный ремонт автомобильной дороги Красноборск – Хмелевская на участке км 0+000 – км 2+757 (устройство электроосвещения) в Красноборском районе Архангельской области. 1 пусковой комплекс",
        ///         "budget": 49041762.00,
        ///         "startDate": "2024-01-01",
        ///         "endDate": "2024-12-31",
        ///         "status": "draft"
        ///     }
        ///
        /// </remarks>
        /// <param name="project">Данные нового проекта</param>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="400">Ошибка валидации данных</response>
        [HttpPost]
        [ProducesResponseType(typeof(ProjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddProject(ProjectDto project)
        {
            try
            {
                return Ok(await _projectService.AddProjectAsync(project));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Возвращает список проектов
        /// </summary>
        /// <remarks>
        /// Примеры запросов:
        ///
        ///     GET /api/projects?status=1
        ///     GET /api/projects?targetDate=2024-05-15
        ///     GET /api/projects?startDateFrom=2024-01-01&amp;startDateTo=2024-06-30
        ///     GET /api/projects?status=2&amp;targetDate=2024-07-01
        ///     
        /// </remarks>
        /// <param name="status">Статус отфильтрованных проектов</param>
        /// <param name="targetDate">Дата, в которую проект будет выполняться (yyyy-MM-dd)</param>
        /// <param name="startDateFrom">Начальная дата диапазона (yyyy-MM-dd)</param>
        /// <param name="startDateTo">Конечная дата диапазона (yyyy-MM-dd)</param>
        /// <response code="200">Успешное выполнение</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProjectDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProjects(
            [FromQuery] Status? status,
            [FromQuery] string? targetDate,
            [FromQuery] string? startDateFrom,
            [FromQuery] string? startDateTo)
        {
                return Ok(await _projectService.GetProjectsAsync(status, targetDate, startDateFrom, startDateTo));
        }

        /// <summary>
        /// Обновляет данные проекта на новые
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     PUT /api/projects
        ///     {
        ///         "idProduct": 1,
        ///         "name": "​Капитальный ремонт автомобильной дороги Красноборск – Хмелевская на участке км 0+000 – км 2+757 (устройство электроосвещения) в Красноборском районе Архангельской области. 1 пусковой комплекс",
        ///         "budget": 49041762.00,
        ///         "startDate": "2024-01-01",
        ///         "endDate": "2024-12-31",
        ///         "status": "draft"
        ///     }
        ///
        /// </remarks>
        /// <param name="project">Новые данные проекта</param>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="400">Ошибка валидации данных</response>
        [HttpPut]
        [ProducesResponseType(typeof(ProjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateProject(ProjectDto project)
        {
            try
            {
                return Ok(await _projectService.UpdateProjectAsync(project));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
