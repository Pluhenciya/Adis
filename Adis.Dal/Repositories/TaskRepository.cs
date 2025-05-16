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
    public class TaskRepository : EFGenericRepository<ProjectTask>, ITaskRepository
    {
        public TaskRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<ProjectTask?> GetTaskDetailsByIdAsync(int id)
        {
            TaskDetailsByIdSpecification spec = new(id);

            return (await GetAsync(spec)).FirstOrDefault();
        }

        public async Task<IEnumerable<ProjectTask>> GetTasksByIdUserAsync(int idUser)
        {
            TaskByIdUserSpecification spec = new(idUser);
            return await GetAsync(spec);
        }
    }
}
