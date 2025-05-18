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
    public class ExecutionTaskRepository : EFGenericRepository<ExecutionTask>, IExecutionTaskRepository
    {
        public ExecutionTaskRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
