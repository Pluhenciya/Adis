using Adis.Bll.Dtos;
using Adis.Bll.Interfaces;
using Adis.Dal.Interfaces;
using Adis.Dm;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Services
{
    public class DocumentService : IDocumentService
    {
        private const string DIRECTORY_PATH = $"documents";
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IDocumentRepository _documentRepository;
        private readonly IMapper _mapper;

        public DocumentService(IHttpContextAccessor contextAccessor, IDocumentRepository documentRepository, IMapper mapper)
        {
            _contextAccessor = contextAccessor;
            _documentRepository = documentRepository;
            _mapper = mapper;
        }

        public async Task<DocumentDto> UploadDocumentAsync(IFormFile file, int? idTask)
        {
            if (!Directory.Exists(DIRECTORY_PATH))
                Directory.CreateDirectory(DIRECTORY_PATH);

            var document = new Document
            {
                FileName = file.FileName,
                DocumentType = DocumentType.Other,
            };

            var user = _contextAccessor.HttpContext.User;

            if (user.IsInRole(Role.Projecter.ToString()))
                document.IdUser = Int32.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            if (idTask.HasValue)
            {
                document.IdTask = idTask.Value;
            }

            var createdDocument = await _documentRepository.AddAsync(document);

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            var uniqueFileName = $"{createdDocument.IdDocument}{fileExtension}";
            var filePath = Path.Combine(DIRECTORY_PATH, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return _mapper.Map<DocumentDto>(createdDocument);
        }

        public async Task<FileStreamResult> DownloadDocumentAsync(int idDocument)
        {
            var document = await _documentRepository.GetByIdAsync(idDocument);
            if (document == null)
                throw new KeyNotFoundException("Document not found");

            var fileExtension = Path.GetExtension(document.FileName).ToLowerInvariant();
            var fileName = $"{document.IdDocument}{fileExtension}";
            var filePath = Path.Combine(DIRECTORY_PATH, fileName);

            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found on server");

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            return new FileStreamResult(stream, GetMimeType(fileExtension))
            {
                FileDownloadName = document.FileName
            };
        }

        private string GetMimeType(string fileExtension)
        {
            return fileExtension switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".zip" => "application/zip",
                _ => "application/octet-stream"
            };
        }
    }
}
