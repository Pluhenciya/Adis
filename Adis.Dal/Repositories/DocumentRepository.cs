using Adis.Dal.Data;
using Adis.Dal.Interfaces;
using Adis.Dal.Specifications;
using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dal.Repositories
{
    public class DocumentRepository : EFGenericRepository<Document>, IDocumentRepository
    {
        public DocumentRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Document>> GetDocumentsByIdProjectAsync(int idProject)
        {
            DocumentsByIdProjectSpecification spec = new(idProject);
            return await GetAsync(spec);
        }

        public async Task<IEnumerable<Document>> GetDocumentsByIdsAsync(IEnumerable<int> IdsDocuments)
        {
            DocumentsByIdsSpecification spec = new(IdsDocuments);
            return await GetAsync(spec);
        }

        public async Task<IEnumerable<Document>> GetGuideDocumentsAsync()
        {
            DocumentsWithoutTaskSpecification spec = new();
            return await GetAsync(spec);
        }
    }
}
