using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dal.Specifications
{
    internal class TaskByIdUserSpecification : Specification<ProjectTask>
    {
        public TaskByIdUserSpecification(int idUser) 
        {
            ApplyCriteria(t => t.Performers.Any(p => p.Id == idUser) || t.Checkers.Any(p => p.Id == idUser));
            AddInclude(t => t.Checkers);
            AddInclude(t => t.Performers);
        }
    }
}
