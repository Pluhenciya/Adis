using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dal.Interfaces
{
    /// <summary>
    /// Позволяет управлять данными проектов
    /// </summary>
    public interface IProjectRepository : IRepository<Project>
    {
        /// <summary>
        /// Возвращет отфильтрованный список проектов
        /// </summary>
        /// <param name="status">Статус отфильтрованных проектов</param>
        /// <param name="targetDate">Дата, в которую проект будет выполнятся</param>
        /// <param name="startDateFrom">Дата начала диапозона поиска проекта</param>
        /// <param name="startDateTo">Дата конца диапозона поиска проекта</param>
        /// <param name="page">Номер страницы для пагинации</param>
        /// <param name="pageSize">Количество записей на страницы</param>
        /// <param name="sortField">Свойство, по которому сортировать</param>
        /// <param name="sortOrder">Сортировать по возрастанию или по убыванию</param>
        /// <returns>Отфильтрованный список проетов и их общее количество</returns>
        Task<(IEnumerable<Project>, int)> GetFilteredProjectsAsync(
        ProjectStatus? status = null,
        DateOnly? targetDate = null,
        DateOnly? startDateFrom = null,
        DateOnly? startDateTo = null,
        string? search = null,
        int? idUser = null,
        string sortField = "StartDate",
        string sortOrder = "desc",
        int page = 1,
        int pageSize = 10);
    }
}
