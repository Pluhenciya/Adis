using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dal.Specifications
{
    public class TaskDetailsByIdSpecification : Specification<ProjectTask>
    {
        public TaskDetailsByIdSpecification(int id)
        {
            ApplyCriteria(t => t.IdTask == id);

            AddInclude(t => t.Checkers);
            AddInclude(t => t.Performers);
            AddInclude(t => t.Documents);
            AddInclude(t => t.Comments);
            AddInclude($"{nameof(ProjectTask.Comments)}.{nameof(Comment.Sender)}");
        }
    }
}
