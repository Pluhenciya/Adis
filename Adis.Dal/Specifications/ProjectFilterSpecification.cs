using Adis.Dm;
using System.Linq.Expressions;

namespace Adis.Dal.Specifications
{
    /// <summary>
    /// Спецификация для фильтрации проектов
    /// </summary>
    public class ProjectFilterSpecification : Specification<Project>
    {
        public ProjectFilterSpecification(
            ProjectStatus? status,
            DateOnly? targetDate,
            DateOnly? startDateFrom,
            DateOnly? startDateTo,
            string? search,
            int? idUser,
            string sortField = "StartDate",
            string sortOrder = "desc",
            int page = 1,
            int pageSize = 10)
        {
            ApplyCriteria(p => !p.IsDeleted);

            if (idUser.HasValue)
            {
                ApplyCriteria(p => p.IdUser == idUser);
            }

            // Фильтр по статусу
            if (status.HasValue)
            {
                ApplyCriteria(p => p.Status == status.Value);
            }

            // Фильтр по конкретной дате
            if (targetDate.HasValue)
            {
                ApplyCriteria(p => p.StartDate <= targetDate.Value
                                && p.EndDate >= targetDate.Value);
            }

            // Фильтр по диапазону дат начала
            if (startDateFrom.HasValue || startDateTo.HasValue)
            {
                if (startDateFrom.HasValue && startDateTo.HasValue)
                {
                    ApplyCriteria(p => p.StartDate >= startDateFrom.Value
                                    && p.StartDate <= startDateTo.Value);
                }
                else if (startDateFrom.HasValue)
                {
                    ApplyCriteria(p => p.StartDate >= startDateFrom.Value);
                }
                else
                {
                    ApplyCriteria(p => p.StartDate <= startDateTo!.Value);
                }
            }

            if (!string.IsNullOrEmpty(search))
                ApplyCriteria(p => p.Name.Contains(search));

            // Сортировка
            var orderExpression = GetOrderExpression(sortField);
            if (orderExpression != null)
            {
                if (sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase))
                    ApplyOrderByDescending(orderExpression);
                else
                    ApplyOrderBy(orderExpression);
            }

            // Пагинация
            ApplyPaging((page - 1) * pageSize, pageSize);

            AddInclude(p => p.WorkObject);
            AddInclude(p => p.User);
            AddInclude(p => p.Tasks);
            AddInclude(p => p.Contractor!);
            AddInclude(p => p.Tasks.Select(t => t.Checkers));
            AddInclude(p => p.Tasks.Select(t => t.Performers));
        }

        /// <summary>
        /// Возвращает лямбда-выражение с указанием свойства для сортировки
        /// </summary>
        /// <param name="sortField">Строковое название свойства</param>
        /// <returns>Лямбда-выражение для сортировки</returns>
        private Expression<Func<Project, object>>? GetOrderExpression(string sortField)
        {
            return sortField?.ToLower() switch
            {
                "name" => p => p.Name,
                "startdate" => p => p.StartDate,
                "idproject" => p => p.IdProject,
                _ => null
            };
        }
    }
}
