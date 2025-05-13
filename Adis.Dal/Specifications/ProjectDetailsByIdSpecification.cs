using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dal.Specifications
{
    public class ProjectDetailsByIdSpecification : Specification<Project>
    {
        public ProjectDetailsByIdSpecification(int id) 
        {
            ApplyCriteria(p => p.IdProject == id);
            AddInclude(p => p.WorkObject);
            AddInclude(p => p.User);
            AddInclude(p => p.Tasks);
            AddInclude(p => p.Contractor!);
            AddInclude($"{nameof(Project.Tasks)}.{nameof(ProjectTask.Checkers)}");
            AddInclude($"{nameof(Project.Tasks)}.{nameof(ProjectTask.Performers)}");
        }
    }
}
