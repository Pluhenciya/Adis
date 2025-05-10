using Adis.Bll.Dtos;
using Adis.Bll.Dtos.Project;
using Adis.Bll.Interfaces;
using Adis.Dal.Interfaces;
using Adis.Dal.Repositories;
using Adis.Dm;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Adis.Bll.Services
{
    /// <inheritdoc cref="IProjectService"/>
    public class ProjectService : IProjectService
    {
        private readonly IMapper _mapper;
        private readonly IProjectRepository _projectRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserService _userService;

        public ProjectService(IMapper mapper, IProjectRepository projectRepository, IHttpContextAccessor httpContextAccessor, IUserService userService)
        {
            _mapper = mapper;
            _projectRepository = projectRepository;
            _contextAccessor = httpContextAccessor;
            _userService = userService;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentException">Возникает когда данные проекта не прошли валидацию</exception>
        public async Task<GetProjectDto> AddProjectAsync(PostProjectDto projectDto)
        {
            await ValidateProjectAsync(projectDto);

            var project = _mapper.Map<Project>(projectDto);

            return _mapper.Map<GetProjectDto>(await _projectRepository.AddAsync(project));
        }

        public async Task<PaginatedResult<GetProjectDto>> GetProjectsAsync(
            ProjectStatus? status,
            string? targetDate,
            string? startDateFrom,
            string? startDateTo,
            string? search,
            int? idUser,
            string sortField = "StartDate",
            string sortOrder = "desc",
            int page = 1,
            int pageSize = 10)
        {
            var parsedTargetDate = ParseDate(targetDate);
            var parsedStartDateFrom = ParseDate(startDateFrom);
            var parsedStartDateTo = ParseDate(startDateTo);

            var (projects, totalCount) = await _projectRepository.GetFilteredProjectsAsync(
                status,
                parsedTargetDate,
                parsedStartDateFrom,
                parsedStartDateTo,
                search,
                idUser,
                sortField,
                sortOrder,
                page,
                pageSize);

            return new PaginatedResult<GetProjectDto>
            {
                Items = _mapper.Map<IEnumerable<GetProjectDto>>(projects),
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentException">Возникает когда данные проекта не прошли валидацию</exception>
        public async Task<GetProjectDto> UpdateProjectAsync(PostProjectDto projectDto)
        {
            try
            {
                var existingProject = await _projectRepository.GetByIdAsync(projectDto.IdProject);

                await ValidateProjectAsync(projectDto);

                _mapper.Map(projectDto, existingProject);

                var updatedProject = await _projectRepository.UpdateAsync(existingProject);

                return _mapper.Map<GetProjectDto>(updatedProject);
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException("Проекта с таким идентификатором не существует");
            }
        }

        private async Task ValidateProjectAsync(PostProjectDto projectDto)
        {
            if (projectDto.StartExecutionDate != null && projectDto.EndExecutionDate != null && projectDto.StartExecutionDate > projectDto.EndExecutionDate)
                throw new ArgumentException("Дата начала выполнения работ не может быть позже чем дата оканчания");

            if (projectDto.StartExecutionDate != null && projectDto.EndDate > projectDto.StartExecutionDate)
                throw new ArgumentException("Дата начала проектирования не может быть позже чем дата оканчания работ");

            if (projectDto.StartDate != null && projectDto.StartDate > projectDto.EndDate)
                throw new ArgumentException("Дата начала проектирования не может быть позже чем дата оканчания");

            if (projectDto.IdWorkObject == 0 && projectDto.WorkObject == null)
                throw new ArgumentException("Локация объекта работ обязательна");

            var user = _contextAccessor.HttpContext.User;

            int idUser = projectDto.IdUser == 0 ? Int32.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value!) : projectDto.IdUser;

            var responsiblePerson = await _userService.GetUserByIdAsync(idUser);

            if (responsiblePerson.Role != Role.ProjectManager)
                throw new ArgumentException("Этот пользователь не может управлять проектом");

            if (projectDto.StartDate == DateOnly.MinValue)
                projectDto.StartDate = DateOnly.FromDateTime(DateTime.Now);

            if (projectDto.IdContractor != null)
                projectDto.ContractorName = null!;

            if (projectDto.IdWorkObject != 0)
                projectDto.WorkObject = null!;

            if((projectDto.Status == ProjectStatus.InExecution
                || projectDto.Status == ProjectStatus.Completed)
                && projectDto.StartExecutionDate == null 
                && projectDto.EndExecutionDate == null
                && (projectDto.ContractorName == null 
                || projectDto.IdContractor == null))
                throw new ArgumentException("Данные выполнения отсутствуют при статусе Исполнение или Завершен");

            if (!user.IsInRole(Role.Admin.ToString()))
            {
                projectDto.StartExecutionDate = null;
                projectDto.EndExecutionDate = null;
                projectDto.ContractorName = null!;
                projectDto.IdContractor = null!;
                projectDto.IdUser = Int32.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                projectDto.StartDate = DateOnly.FromDateTime(DateTime.Now);
                projectDto.Status = ProjectStatus.Designing;
            }
        }

        /// <summary>
        /// Парсит строку в дату
        /// </summary>
        /// <param name="dateString">Дата в строке</param>
        /// <returns>Дата типа DateOnly</returns>
        private DateOnly? ParseDate(string? dateString)
        {
            if (string.IsNullOrEmpty(dateString))
                return null;

            return DateOnly.ParseExact(dateString, "yyyy-MM-dd");
        }

        public async Task DeleteProjectAsync(int id)
        {
            var user = _contextAccessor.HttpContext.User;
            if (user.IsInRole(Role.Admin.ToString()))
                await _projectRepository.DeleteAsync(id);
            else
            {
                var project = await _projectRepository.GetByIdAsync(id);
                project.IsDeleted = true;
                await _projectRepository.UpdateAsync(project);
            }
        }
    }
}
