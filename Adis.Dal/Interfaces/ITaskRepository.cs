using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dal.Interfaces
{
    public interface ITaskRepository : IRepository<ProjectTask>
    {
        public Task<ProjectTask?> GetTaskDetailsByIdAsync(int id);
    }
}
