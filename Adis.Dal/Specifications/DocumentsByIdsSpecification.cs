using Adis.Dal.Migrations;
using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dal.Specifications
{
    public class DocumentsByIdsSpecification : Specification<Document>
    {
        public DocumentsByIdsSpecification(IEnumerable<int> IdsDocuments) 
        {
            ApplyCriteria(d => IdsDocuments.Contains(d.IdDocument));
        }
    }
}
