using Adis.Bll.Dtos.Task;
using Adis.Bll.Interfaces;
using Adis.Dal.Interfaces;
using Adis.Dal.Repositories;
using Adis.Dm;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Services
{
    public class TaskService : ITaskService
    {
        private readonly IMapper _mapper;
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;

        public TaskService(IMapper mapper, ITaskRepository taskRepository, IUserRepository userRepository) 
        {
            _mapper = mapper;
            _taskRepository = taskRepository;
            _userRepository = userRepository;
        }

        public async Task<TaskDetailsDto> AddTaskAsync(PostTaskDto taskDto)
        {
            var task = _mapper.Map<ProjectTask>(taskDto);
            await AddUsersForTask(taskDto.IdCheckers, taskDto.IdPerformers, task);

            int idTask = (await _taskRepository.AddAsync(task)).IdTask;

            return (await GetTaskDetailsByIdAsync(idTask))!;
        }

        private async Task AddUsersForTask(IEnumerable<int> idCheckers, IEnumerable<int> idPerformers, ProjectTask task)
        {
            var checkers = new List<User>();
            foreach (var id in idCheckers)
            {
                checkers.Add(await _userRepository.GetByIdAsync(id));
            }
            task.Checkers = checkers;

            var performers = new List<User>();
            foreach (var id in idPerformers)
            {
                performers.Add(await _userRepository.GetByIdAsync(id));
            }
            task.Performers = performers;
        }

        public async Task<TaskDetailsDto?> GetTaskDetailsByIdAsync(int id)
        {
            return _mapper.Map<TaskDetailsDto>(await _taskRepository.GetTaskDetailsByIdAsync(id));
        }

        public async Task<TaskDetailsDto> UpdateTaskAsync(PutTaskDto taskDto)
        {
            var existingTask = await _taskRepository.GetTaskDetailsByIdAsync(taskDto.IdTask);

            _mapper.Map(taskDto, existingTask);
            await AddUsersForTask(taskDto.IdCheckers, taskDto.IdPerformers, existingTask);

            int idTask = (await _taskRepository.UpdateAsync(existingTask)).IdTask;

            return (await GetTaskDetailsByIdAsync(idTask))!;
        }
    }
}
