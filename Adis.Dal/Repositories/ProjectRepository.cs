using Adis.Dal.Data;
using Adis.Dal.Interfaces;
using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dal.Repositories
{
    /// <inheritdoc cref="IProjectRepository"/>
    public class ProjectRepository : EFGenericRepository<Project>, IProjectRepository
    {
        public ProjectRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
