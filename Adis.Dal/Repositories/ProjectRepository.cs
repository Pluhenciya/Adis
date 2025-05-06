using Adis.Dal.Data;
using Adis.Dal.Interfaces;
using Adis.Dal.Specifications;
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

        public async Task<(IEnumerable<Project>, int)> GetFilteredProjectsAsync(
            ProjectStatus? status = null,
            DateOnly? targetDate = null,
            DateOnly? startDateFrom = null,
            DateOnly? startDateTo = null,
            string sortField = "StartDate",
            string sortOrder = "desc",
            int page = 1,
            int pageSize = 10)
        {
            var spec = new ProjectFilterSpecification(status, targetDate, startDateFrom, startDateTo, sortField, sortOrder, page, pageSize);

            return (await GetAsync(spec), (await GetAsync()).Count());
        }
    }
}
