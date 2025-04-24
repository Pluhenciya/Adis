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
        /// <param name="startDateFrom">Дата начала диапозона поиска проета</param>
        /// <param name="startDateTo">Дата конца диапозона поиска проекта</param>
        /// <returns>Отфильтрованный список проетов</returns>
        public Task<IEnumerable<Project>> GetFilteredProjectsAsync(
            Status? status = null,
            DateOnly? targetDate = null,
            DateOnly? startDateFrom = null,
            DateOnly? startDateTo = null);
    }
}
