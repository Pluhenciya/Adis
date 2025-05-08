using Adis.Bll.Dtos;
using Adis.Bll.Dtos.Project;
using Adis.Bll.Interfaces;
using Adis.Dal.Interfaces;
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
        public async Task<PostProjectDto> AddProjectAsync(PostProjectDto projectDto)
        {
            if (DateOnly.FromDateTime(DateTime.Now) > projectDto.EndDate)
                throw new ArgumentException("Дата оканчания не может быть в прошлом");

            var project = _mapper.Map<Project>(projectDto);
            var user = _contextAccessor.HttpContext.User;

            int idUser = project.IdUser == 0 ? Int32.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value!) : project.IdUser;

            var responsiblePerson = await _userService.GetUserByIdAsync(idUser);

            if (responsiblePerson.Role != Role.ProjectManager)
                throw new ArgumentException("Этот пользователь не может управлять проектом");

            if (project.StartDate == DateOnly.MinValue)
                project.StartDate = DateOnly.FromDateTime(DateTime.Now);


            if (project.IdLocation != 0)
                project.Location = null!;

            if (!user.IsInRole(Role.Admin.ToString()))
            {
                project.IdUser = idUser;
                project.StartDate = DateOnly.FromDateTime(DateTime.Now);
                project.Status = ProjectStatus.Designing;
            }

            return _mapper.Map<PostProjectDto>(await _projectRepository.AddAsync(project));
        }

        public async Task<PaginatedResult<GetProjectDto>> GetProjectsAsync(
            ProjectStatus? status,
            string? targetDate,
            string? startDateFrom,
            string? startDateTo,
            string? search,
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
        public async Task<PostProjectDto> UpdateProjectAsync(PostProjectDto project)
        {
            try
            {
                var existingProject = await _projectRepository.GetByIdAsync(project.IdProject);

                if (DateOnly.FromDateTime(DateTime.Now) > project.EndDate)
                    throw new ArgumentException("Дата оканчания не может быть в прошлом");

                _mapper.Map(project, existingProject);

                var updatedProject = await _projectRepository.UpdateAsync(existingProject);

                return _mapper.Map<PostProjectDto>(updatedProject);
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException("Проекта с таким идентификатором не существует");
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
    }
}
