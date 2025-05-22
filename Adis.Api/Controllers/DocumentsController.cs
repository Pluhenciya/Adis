using Adis.Bll.Interfaces;
using Adis.Dm;
using DocumentFormat.OpenXml.Office2010.Excel;
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
        public async Task<IActionResult> UploadDocument(IFormFile file, [FromQuery] int? idTask = null, [FromQuery] DocumentType? documentType = null)
        {
            return Ok(await _documentService.UploadDocumentAsync(file, idTask, documentType));
        }

        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadDocument(int id)
        {
            return await _documentService.DownloadDocumentAsync(id);
        }

        [HttpGet("{idProject}")]
        public async Task<IActionResult> GetDocumentsByIdProject(int idProject)
        {
            return Ok(await _documentService.GetDocumentsAsyncByIdProjectAsync(idProject));
        }

        [HttpGet("download-zip")]
        public async Task<IActionResult> DownloadZipDocuments([FromQuery] string ids)
        {
            try
            {
                return await _documentService.DownloadZipDocumentsAsync(ids);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("guide")]
        public async Task<IActionResult> GetGuideDocuments()
        {
            return Ok(await _documentService.GetGuideDocumentsAsync());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            await _documentService.DeleteDocumentAsync(id);
            return Ok();
        }
    }
}
