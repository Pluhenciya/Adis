using Adis.Bll.Dtos;
using Adis.Bll.Dtos.Project;
using Adis.Bll.Interfaces;
using Adis.Dm;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Adis.Api.Controllers
{
    /// <summary>
    /// Позволяет управлять проектами
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController(IProjectService projectService) : ControllerBase
    {
        /// <inheritdoc cref="IProjectService"/>
        private readonly IProjectService _projectService = projectService;

        /// <summary>
        /// Добавляет новый проект
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     POST /api/projects
        ///     {
        ///         "name": "Реконструкция автодороги М-5",
        ///         "startDate": "2025-03-01",
        ///         "endDate": "2026-12-31",
        ///         "status": "Completed",
        ///         "idUser": 1
        ///         "workObject": {
        ///             "idLocation": 10,
        ///             "geometry": {
        ///                 "type": "LineString",             
        ///                 "coordinates": [                    
        ///                     [55.796127, 49.106414],           
        ///                     [55.802345, 49.115200]            
        ///                 ]
        ///             },
        ///             "name": "Участок автодороги М-5 (Казань)"
        ///         },
        ///         "contractorName": "Дорстройэксперт",
        ///         "startExecutionDate": "2026-04-01",
        ///         "endExecutionDate": "2027-11-30"
        ///     }
        ///
        /// </remarks>
        /// <param name="project">Данные нового проекта</param>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="400">Ошибка валидации данных</response>
        /// <response code="401">Неавторизованный пользователь</response>
        /// <response code="403">Пользователь без прав на это действие</response>
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost]
        [ProducesResponseType(typeof(GetProjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public async Task<IActionResult> AddProject(PostProjectDto project)
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
        /// Возвращает список проектов с общим количеством и информации о страницах
        /// </summary>
        /// <remarks>
        /// Примеры запросов:
        ///
        ///     GET /api/projects?status=Designing
        ///     GET /api/projects?targetDate=2024-05-15
        ///     GET /api/projects?startDateFrom=2024-01-01&amp;startDateTo=2024-06-30
        ///     GET /api/projects?status=Designing&amp;targetDate=2024-07-01
        ///     
        /// </remarks>
        /// <param name="status">Статус отфильтрованных проектов</param>
        /// <param name="targetDate">Дата, в которую проект будет выполняться (yyyy-MM-dd)</param>
        /// <param name="startDateFrom">Начальная дата диапазона (yyyy-MM-dd)</param>
        /// <param name="startDateTo">Конечная дата диапазона (yyyy-MM-dd)</param>
        /// <param name="search">Часть имени наименования проекта для поиска</param>
        /// <param name="idUser">Пользователь, которому принадлежит проект</param>
        /// <param name="page">Номер страницы для пагинации</param>
        /// <param name="pageSize">Количество записей на страницы</param>
        /// <param name="sortField">Свойство, по которому сортировать</param>
        /// <param name="sortOrder">Сортировать по возрастанию или по убыванию</param>
        /// <response code="200">Успешное выполнение</response>
        [HttpGet]
        [ProducesResponseType(typeof(ProjectsResponseDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProjects(
            [FromQuery] ProjectStatus? status,
            [FromQuery] string? targetDate,
            [FromQuery] string? startDateFrom,
            [FromQuery] string? startDateTo,
            [FromQuery] string? search,
            [FromQuery] int? idUser,
            [FromQuery] string sortField = "StartDate",
            [FromQuery] string sortOrder = "desc",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var result = await _projectService.GetProjectsAsync(
                status,
                targetDate,
                startDateFrom,
                startDateTo,
                search,
                idUser,
                sortField,
                sortOrder,
                page,
                pageSize);

            return Ok(new ProjectsResponseDto
            {
                Projects = result.Items,
                TotalCount = result.TotalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        /// <summary>
        /// Обновляет данные проекта на новые
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     PUT /api/projects
        ///     {
        ///         "name": "Реконструкция автодороги М-5",
        ///         "startDate": "2025-03-01",
        ///         "endDate": "2026-12-31",
        ///         "status": "Completed",
        ///         "idUser": 1
        ///         "workObject": {
        ///             "idLocation": 10,
        ///             "geometry": {
        ///                 "type": "LineString",             
        ///                 "coordinates": [                    
        ///                     [55.796127, 49.106414],           
        ///                     [55.802345, 49.115200]            
        ///                 ]
        ///             },
        ///             "name": "Участок автодороги М-5 (Казань)"
        ///         },
        ///         "contractorName": "Дорстройэксперт",
        ///         "startExecutionDate": "2026-04-01",
        ///         "endExecutionDate": "2027-11-30"
        ///     }
        ///
        /// </remarks>
        /// <param name="project">Новые данные проекта</param>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="400">Ошибка валидации данных</response>
        /// <response code="401">Неавторизованный пользователь</response>
        /// <response code="403">Пользователь без прав на это действие</response>
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPut]
        [ProducesResponseType(typeof(GetProjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public async Task<IActionResult> UpdateProject(PostProjectDto project)
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

        /// <summary>
        /// Удаляет проект по идентификатору
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     DELETE /api/projects/1
        ///     
        /// </remarks>
        /// <param name="id">Идентификатор удаляемого проекта</param>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="404">Проект с данным идентификатором не найден</response>
        /// <response code="401">Неавторизованный пользователь</response>
        /// <response code="403">Пользователь без прав на это действие</response>
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public async Task<IActionResult> DeleteProject(int id)
        {
            try
            {
                await _projectService.DeleteProjectAsync(id);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Возвращает проект по идентификатору
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     GET /api/projects/1
        ///     
        /// </remarks>
        /// <param name="id">Идентификатор искомого проекта</param>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="404">Проект с данным идентификатором не найден</response>
        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetProjectDetailsById(int id)
        {
            var project = await _projectService.GetProjectDetailsByIdAsync(id);
            if (project == null)
                return NotFound("Проект с таким id не найден");
            return Ok(project);
        }
    }
}
