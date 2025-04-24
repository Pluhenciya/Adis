using Adis.Bll.Dtos;
using Adis.Bll.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Adis.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService) 
        {
            _projectService = projectService;
        }

        [HttpPost]
        public async Task<IActionResult> AddProject(ProjectDto project) 
        {
            return Ok(await _projectService.AddProject(project));
        }
    }
}
