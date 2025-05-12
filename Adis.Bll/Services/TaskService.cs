using Adis.Bll.Dtos.Task;
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
    public class TaskService : ITaskService
    {
        private readonly IMapper _mapper;
        private readonly ITaskRepository _taskRepository;
        private readonly IUserService _userService;

        public TaskService(IMapper mapper, ITaskRepository taskRepository, IUserService userService) 
        {
            _mapper = mapper;
            _taskRepository = taskRepository;
            _userService = userService;
        }

        public async Task<TaskDetailsDto> AddTaskAsync(PostTaskDto taskDto)
        {
            var task = _mapper.Map<ProjectTask>(taskDto);

            var checkers = new List<User>();
            foreach (var id in taskDto.IdCheckers)
            {
                checkers.Add(_mapper.Map<User>(_userService.GetUserByIdAsync(id)));
            }
            task.Checkers = checkers;

            var performers = new List<User>();
            foreach (var id in taskDto.IdPerformers)
            {
                checkers.Add(_mapper.Map<User>(_userService.GetUserByIdAsync(id)));
            }
            task.Performers = performers;

            int idTask = (await _taskRepository.AddAsync(task)).IdTask;

            return (await GetTaskDetailsByIdAsync(idTask))!;
        }

        public async Task<TaskDetailsDto?> GetTaskDetailsByIdAsync(int id)
        {
            return _mapper.Map<TaskDetailsDto>(await _taskRepository.GetTaskDetailsByIdAsync(id));
        }
    }
}
