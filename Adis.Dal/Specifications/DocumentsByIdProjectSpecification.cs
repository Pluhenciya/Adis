using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dal.Specifications
{
    public class DocumentsByIdProjectSpecification : Specification<Document>
    {
        public DocumentsByIdProjectSpecification(int idProject) 
        {
            AddInclude(d => d.Task!);
            ApplyCriteria(d => d.Task != null && d.Task.IdProject == idProject);
        }
    }
}
