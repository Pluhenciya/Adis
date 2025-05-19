using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dal.Interfaces
{
    public interface IDocumentRepository : IRepository<Document>
    {
        public Task<IEnumerable<Document>> GetDocumentsByIdProjectAsync(int idProject);

        public Task<IEnumerable<Document>> GetDocumentsByIdsAsync(IEnumerable<int> IdsDocuments);
    }
}
