using Adis.Bll.Dtos;
using Adis.Bll.Interfaces;
using Adis.Dal.Interfaces;
using Adis.Dm;
using AutoMapper;
using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IDocumentRepository _documentRepository;
        private readonly IMapper _mapper;
        private readonly IWorkObjectSectionService _workObjectSectionService;
        private readonly IMemoryCache _memoryCache;
        private readonly INeuralGuideService _neuralGuideService;

        public string DirectoryPath => "documents";

        public DocumentService(IHttpContextAccessor contextAccessor, IDocumentRepository documentRepository, IMapper mapper, IWorkObjectSectionService workObjectSectionService, IMemoryCache memoryCache, INeuralGuideService neuralGuideService)
        {
            _contextAccessor = contextAccessor;
            _documentRepository = documentRepository;
            _mapper = mapper;
            _workObjectSectionService = workObjectSectionService;
            _memoryCache = memoryCache;
            _neuralGuideService = neuralGuideService;
        }

        public async Task<DocumentDto> UploadDocumentAsync(IFormFile file, int? idTask, DocumentType? documentType)
        {
            if (!Directory.Exists(DirectoryPath))
                Directory.CreateDirectory(DirectoryPath);

            var document = new Document
            {
                FileName = file.FileName,
                DocumentType = documentType ?? DocumentType.Other,
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
            var filePath = Path.Combine(DirectoryPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            if (!idTask.HasValue)
            {     
                _memoryCache.Remove("documents");
                Task.Run(() =>
                    SafeInitializeNeuralGuideAsync()
                   .ContinueWith(t =>
                   {
                       if (t.IsFaulted)
                       {
                           Console.WriteLine($"Ошибка фоновой инициализации: {t.Exception}");
                       }
                   }));
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
            var filePath = Path.Combine(DirectoryPath, fileName);

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

        public async Task<IEnumerable<ExecutionTask>> SelectEstimateFromProjectAsync(int idDocument, int idProject)
        {
            var document = await _documentRepository.GetByIdAsync(idDocument);
            document.DocumentType = DocumentType.Estimate;
            await _documentRepository.UpdateAsync(document);

            var fileExtension = Path.GetExtension(document.FileName).ToLowerInvariant();
            var fileName = $"{document.IdDocument}{fileExtension}";
            var filePath = Path.Combine(DirectoryPath, fileName);
            var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheet(1);
            var rows = worksheet.RangeUsed()!.RowsUsed();

            WorkObjectSection workObjectSection = new();
            var executionTasks = new List<ExecutionTask>();
            foreach (var row in rows.Skip(5))
            {
                var text = row.Cell(1).Value.Type;
                if (row.Cell(2).Value.IsText && row.Cell(2).Value.GetText().ToLower().Contains("участок") && row.Cell(6).IsEmpty())
                {
                    workObjectSection = new WorkObjectSection
                    {
                        Name = row.Cell(2).Value.GetText()
                    };
                    workObjectSection = await _workObjectSectionService.AddWorkObjectSectionAsync(workObjectSection);
                }
                if (row.Cell(1).Value.IsNumber && double.IsInteger(row.Cell(1).Value.GetNumber()))
                {
                    executionTasks.Add(new ExecutionTask()
                    {
                        Name = row.Cell(2).Value.GetText(),
                        IdProject = idProject,
                        IdWorkObjectSection = workObjectSection.IdWorkObjectSection,
                    });
                }
            }
            return executionTasks;
        }

        public async Task<IEnumerable<DocumentDto>> GetDocumentsAsyncByIdProjectAsync(int idProject)
        {
            return _mapper.Map<IEnumerable<DocumentDto>>(await _documentRepository.GetDocumentsByIdProjectAsync(idProject));
        }

        public async Task<FileStreamResult> DownloadZipDocumentsAsync(string documentIds)
        {
            string tempZipPath = null;
            try
            {
                var idArray = documentIds.Split(',')
                    .Select(id => int.TryParse(id, out var num) ? num : (int?)null)
                    .Where(id => id.HasValue)
                    .Select(id => id.Value)
                    .ToArray();

                if (idArray.Length == 0)
                    throw new ArgumentException("Invalid document IDs format");

                var documents = await _documentRepository.GetDocumentsByIdsAsync(idArray);

                if (!documents.Any())
                    throw new FileNotFoundException("No documents found");

                // Генерируем уникальное имя файла
                tempZipPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

                using (var zipArchive = ZipFile.Open(tempZipPath, ZipArchiveMode.Create))
                {
                    foreach (var doc in documents)
                    {
                        var fileExtension = Path.GetExtension(doc.FileName).ToLowerInvariant();
                        var fileName = $"{doc.IdDocument}{fileExtension}";
                        var filePath = Path.Combine(DirectoryPath, fileName);

                        if (!File.Exists(filePath)) continue;

                        zipArchive.CreateEntryFromFile(filePath, doc.FileName);
                    }
                }

                var stream = new FileStream(tempZipPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.DeleteOnClose);

                return new FileStreamResult(stream, "application/zip")
                {
                    FileDownloadName = $"documents_{DateTime.Now:yyyyMMddHHmmss}.zip"
                };
            }
            catch
            {
                // Удаляем временный файл в случае ошибки
                if (tempZipPath != null && File.Exists(tempZipPath))
                {
                    File.Delete(tempZipPath);
                }
                throw;
            }
        }

        public async Task DeleteDocumentAsync(int id)
        {
            var document = await _documentRepository.GetByIdAsync(id);
            if (document == null)
                throw new KeyNotFoundException("Document not found");

            var fileExtension = Path.GetExtension(document.FileName).ToLowerInvariant();
            var fileName = $"{document.IdDocument}{fileExtension}";
            var filePath = Path.Combine(DirectoryPath, fileName);

            if (!File.Exists(filePath))
            {
                await _documentRepository.DeleteAsync(id);
                throw new FileNotFoundException("File not found on server");
            }
                

            File.Delete(filePath);

            await _documentRepository.DeleteAsync(id);
           
            _memoryCache.Remove("documents");
            await Task.Run(async () =>
            {
                // Получаем актуальные документы через новый контекст
                var docs = await GetGuideDocumentsAsync();
                await _neuralGuideService.InitializeAsync(docs, DirectoryPath);
            });
        }

        private async Task SafeInitializeNeuralGuideAsync()
        {
            await _neuralGuideService.InitializeAsync(await GetGuideDocumentsAsync(), DirectoryPath);
        }

        public async Task<IEnumerable<DocumentDto>> GetGuideDocumentsAsync()
        {
            return _mapper.Map<IEnumerable<DocumentDto>>(await _documentRepository.GetGuideDocumentsAsync());
        }
    }
}
