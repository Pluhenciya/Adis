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
            string? search = null,
            int? idUser = null,
            string sortField = "StartDate",
            string sortOrder = "desc",
            int page = 1,
            int pageSize = 10)
        {
            var spec = new ProjectFilterSpecification(status, targetDate, startDateFrom, startDateTo, search, idUser, sortField, sortOrder, page, pageSize);

            var projects = await GetAsync(spec);

            spec.DisablePaging();

            var projectsCount = ApplySpecification(spec).Count();

            return (projects, projectsCount);
        }

        public async Task<Project?> GetProjectDetailsByIdAsync(int id)
        {
            var spec = new ProjectDetailsByIdSpecification(id);
            return (await GetAsync(spec)).FirstOrDefault();
        }
    }
}
