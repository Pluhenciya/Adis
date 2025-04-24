using Adis.Bll.Dtos;
using Adis.Bll.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        ///         "endDate": "2024-12-31"
        ///     }
        ///
        /// </remarks>
        /// <param name="project">Данные нового проекта</param>
        [HttpPost]
        public async Task<IActionResult> AddProject(ProjectDto project)
        {
            try
            {
                return Ok(await _projectService.AddProject(project));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Внутренняя ошибка сервера: " + ex.Message);
            }
        }
    }
}
