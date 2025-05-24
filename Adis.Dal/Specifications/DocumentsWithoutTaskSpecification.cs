using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dal.Specifications
{
    public class DocumentsWithoutTaskSpecification : Specification<Document>
    {
        public DocumentsWithoutTaskSpecification() 
        {
            ApplyCriteria(d => d.IdTask == null);
        }
    }
}
