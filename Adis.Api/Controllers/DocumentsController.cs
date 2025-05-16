using Adis.Bll.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Adis.Api.Controllers
{
    /// <summary>
    /// Позволяет управлять документами
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController(IDocumentService documentService) : ControllerBase
    {
        private readonly IDocumentService _documentService = documentService;

        [HttpPost("upload")]
        [Authorize(Roles = "Admin, Projecter")]
        public async Task<IActionResult> UploadDocument(IFormFile file, [FromQuery] int? idTask)
        {
            return Ok(await _documentService.UploadDocumentAsync(file, idTask));
        }

        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadDocument(int id)
        {
            return await _documentService.DownloadDocumentAsync(id);
        }
    }
}
