using Adis.Bll.Dtos.Task;
using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Interfaces
{
    public interface ITaskService
    {
        public Task<TaskDetailsDto?> GetTaskDetailsByIdAsync(int id);

        public Task<TaskDetailsDto> AddTaskAsync(PostTaskDto task);

        public Task<TaskDetailsDto> UpdateTaskAsync(PutTaskDto taskDto);
    }
}
