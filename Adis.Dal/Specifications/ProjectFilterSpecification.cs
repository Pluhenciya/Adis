using Adis.Dal.Interfaces;
using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dal.Specifications
{
    /// <summary>
    /// Спецификация для фильтрации проектов
    /// </summary>
    public class ProjectFilterSpecification : Specification<Project>
    {
        public ProjectFilterSpecification(
            Status? status,
            DateOnly? targetDate,
            DateOnly? startDateFrom,
            DateOnly? startDateTo,
            string sortField = "StartDate",
            string sortOrder = "desc",
            int page = 1,
            int pageSize = 10)
        {
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
        }

        private Expression<Func<Project, object>>? GetOrderExpression(string sortField)
        {
            return sortField?.ToLower() switch
            {
                "name" => p => p.Name,
                "startdate" => p => p.StartDate,
                "budget" => p => p.Budget,
                _ => null
            };
        }
    }
}
