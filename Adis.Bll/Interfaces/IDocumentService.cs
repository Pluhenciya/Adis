using Adis.Bll.Dtos;
using Adis.Dm;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Interfaces
{
    public interface IDocumentService
    {
        public Task<DocumentDto> UploadDocumentAsync(IFormFile file, int? idTask = null, DocumentType? documentType = null);

        public Task<FileStreamResult> DownloadDocumentAsync(int idDocument);

        public Task<IEnumerable<ExecutionTask>> SelectEstimateFromProjectAsync(int idDocument, int idProject);

        public Task<IEnumerable<DocumentDto>> GetDocumentsAsyncByIdProjectAsync(int idProject);

        public Task<FileStreamResult> DownloadZipDocumentsAsync(string documentIds);

        public Task<IEnumerable<DocumentDto>> GetGuideDocumentsAsync();
    }
}
