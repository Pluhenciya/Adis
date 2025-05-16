using Adis.Bll.Dtos;
using Adis.Bll.Interfaces;
using Adis.Dal.Interfaces;
using Adis.Dm;
using AutoMapper;
using Microsoft.AspNetCore.Http;
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
    }
}
