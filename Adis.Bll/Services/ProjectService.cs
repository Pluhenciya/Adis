using Adis.Bll.Dtos;
using Adis.Bll.Interfaces;
using Adis.Dal.Interfaces;
using Adis.Dm;
using AutoMapper;

namespace Adis.Bll.Services
{
    /// <inheritdoc cref="IProjectService"/>
    public class ProjectService : IProjectService
    {
        private readonly IMapper _mapper;
        private readonly IProjectRepository _projectRepository;

        public ProjectService(IMapper mapper, IProjectRepository projectRepository)
        {
            _mapper = mapper;
            _projectRepository = projectRepository;
        }

        public async Task<ProjectDto> AddProjectAsync(ProjectDto project)
        {
            if (project.Budget < 0)
                throw new ArgumentException("Бюджет не может быть отрицательным");
            if (project.StartDate > project.EndDate)
                throw new ArgumentException("Дата оканчания не может быть меньше чем дата начала");
            return _mapper.Map<ProjectDto>(await _projectRepository.AddAsync(_mapper.Map<Project>(project)));
        }

        public async Task<IEnumerable<ProjectDto>> GetProjectsAsync(Status? status, string? targetDate, string? startDateFrom, string? startDateTo)
        {
            // Парсинг дат из query-параметров
            DateOnly? parsedTargetDate = ParseDate(targetDate);
            DateOnly? parsedStartDateFrom = ParseDate(startDateFrom);
            DateOnly? parsedStartDateTo = ParseDate(startDateTo);

            var projects = await _projectRepository.GetFilteredProjectsAsync(
                status,
                parsedTargetDate,
                parsedStartDateFrom,
                parsedStartDateTo);

            return _mapper.Map<IEnumerable<ProjectDto>>(projects);
        }

        public async Task<ProjectDto> UpdateProjectAsync(ProjectDto project)
        {
            try
            {
                var existingProject = await _projectRepository.GetByIdAsync(project.IdProduct);

                if (project.Budget < 0)
                    throw new ArgumentException("Бюджет не может быть отрицательным");
                if (project.StartDate > project.EndDate)
                    throw new ArgumentException("Дата оканчания не может быть меньше чем дата начала");

                _mapper.Map(project, existingProject);

                var updatedProject = await _projectRepository.UpdateAsync(existingProject);

                return _mapper.Map<ProjectDto>(updatedProject);
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
