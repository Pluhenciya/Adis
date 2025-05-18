using Adis.Bll.Interfaces;
using Adis.Dal.Interfaces;
using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Services
{
    public class ExecutionTaskService : IExecutionTaskService
    {
        private readonly IExecutionTaskRepository _executionTaskRepository;

        public ExecutionTaskService(IExecutionTaskRepository executionTaskRepository) 
        {
            _executionTaskRepository = executionTaskRepository;
        }

        public async Task AddExecutionTasksAsync(IEnumerable<ExecutionTask> tasks)
        {
            foreach (var task in tasks)
            {
                await _executionTaskRepository.AddAsync(task);
            }
        }
    }
}
