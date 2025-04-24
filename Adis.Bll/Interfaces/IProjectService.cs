using Adis.Bll.Dtos;
using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Interfaces
{
    /// <summary>
    /// Позволяет управлять проектами
    /// </summary>
    public interface IProjectService
    {
        /// <summary>
        /// Добавляет новый проект
        /// </summary>
        /// <param name="project">Данные нового проекта</param>
        /// <returns>Созданный проект</returns>
        public Task<ProjectDto> AddProject(ProjectDto project);

        /// <summary>
        /// Возвращает список проектов
        /// </summary>
        /// <returns>Список проектов</returns>
        public Task<IEnumerable<ProjectDto>> GetProjects(Status? status, string? targetDate, string? startDateFrom, string? startDateTo);
    }
}
