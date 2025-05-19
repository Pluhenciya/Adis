using Adis.Bll.Dtos;
using Adis.Bll.Interfaces;
using Adis.Dal.Interfaces;
using Adis.Dm;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public ExecutionTaskService(IExecutionTaskRepository executionTaskRepository, IMapper mapper) 
        {
            _executionTaskRepository = executionTaskRepository;
            _mapper = mapper;
        }

        public async Task AddExecutionTasksAsync(IEnumerable<ExecutionTask> tasks)
        {
            foreach (var task in tasks)
            {
                await _executionTaskRepository.AddAsync(task);
            }
        }

        public async Task<ExecutionTaskDto> UpdateExecutionTaskStatus(int idTask, bool isCompleted)
        {
            var task = await _executionTaskRepository.GetByIdAsync(idTask);
            task.IsCompleted = isCompleted;
            var updatedTask = await _executionTaskRepository.UpdateAsync(task);
            return _mapper.Map<ExecutionTaskDto>(updatedTask);
        }
    }
}
