using Adis.Bll.Dtos;
using Adis.Bll.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Adis.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExecutionTasksController : ControllerBase
    {
        private readonly IExecutionTaskService _executionTaskService;

        public ExecutionTasksController(IExecutionTaskService executionTaskService) 
        {
            _executionTaskService = executionTaskService;
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateExecutionTaskStatus(int id, [FromBody] UpdateExecutionTaskStatusDto dto)
        {
            return Ok(await _executionTaskService.UpdateExecutionTaskStatus(id, dto.IsCompleted));
        }
    }
}
