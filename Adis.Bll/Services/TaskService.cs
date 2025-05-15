using Adis.Bll.Dtos.Task;
using Adis.Bll.Interfaces;
using Adis.Dal.Interfaces;
using Adis.Dal.Repositories;
using Adis.Dm;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Services
{
    public class TaskService : ITaskService
    {
        private readonly IMapper _mapper;
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _contextAccessor;

        public TaskService(IMapper mapper, ITaskRepository taskRepository, IUserRepository userRepository, IHttpContextAccessor contextAccessor) 
        {
            _mapper = mapper;
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            _contextAccessor = contextAccessor;
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

        public async Task<bool> TaskExistAsync(int id)
        {
            try
            {
                await _taskRepository.GetByIdAsync(id);
                return true;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        public async Task<IEnumerable<TaskDto>> GetTaskForProjecterAsync()
        {
            int idUser = Int32.Parse(_contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            return _mapper.Map<IEnumerable<TaskDto>>(await _taskRepository.GetTasksByIdUserAsync(idUser));
        }

        public async Task<TaskDto> UpdateTaskStatusAsync(int id, string status)
        {
            if(!Status.TryParse(typeof(Status), status, out var verifedStatus))
                new ArgumentException("Такого статуса нету");
            var task = await _taskRepository.GetByIdAsync(id);
            task.Status = (Status)verifedStatus!;
            return _mapper.Map<TaskDto>(await _taskRepository.UpdateAsync(task));
        }
    }
}
