using Adis.Bll.Dtos;
using Adis.Bll.Dtos.Project;
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
        public Task<GetProjectDto> AddProjectAsync(PostProjectDto project);

        /// <summary>
        /// Возвращает список проектов
        /// </summary>
        /// <param name="status">Статус отфильтрованных проектов</param>
        /// <param name="targetDate">Дата, в которую проект будет выполнятся</param>
        /// <param name="startDateFrom">Дата начала диапозона поиска проекта</param>
        /// <param name="startDateTo">Дата конца диапозона поиска проекта</param>
        /// <param name="page">Номер страницы для пагинации</param>
        /// <param name="pageSize">Количество записей на страницы</param>
        /// <param name="sortField">Свойство, по которому сортировать</param>
        /// <param name="sortOrder">Сортировать по возрастанию или по убыванию</param>
        /// <returns>Список проектов</returns>
        public Task<PaginatedResult<GetProjectDto>> GetProjectsAsync(
        ProjectStatus? status,
        string? targetDate,
        string? startDateFrom,
        string? startDateTo,
        string? search,
        int? idUser,
        string sortField = "StartDate",
        string sortOrder = "desc",
        int page = 1,
        int pageSize = 10);

        /// <summary>
        /// Заполняет проект новыми данными
        /// </summary>
        /// <param name="project">Новые данные проекта</param>
        /// <returns>Изменненый проект</returns>
        public Task<GetProjectDto> UpdateProjectAsync(PostProjectDto project);

        public Task DeleteProjectAsync(int id);

        public Task<GetProjectWithTasksDto?> GetProjectDetailsByIdAsync(int id);
    }
}
