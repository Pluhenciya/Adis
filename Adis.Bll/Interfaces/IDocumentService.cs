using Adis.Bll.Dtos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Interfaces
{
    public interface IDocumentService
    {
        public Task<DocumentDto> UploadDocumentAsync(IFormFile file, int? idTask = null);
    }
}
