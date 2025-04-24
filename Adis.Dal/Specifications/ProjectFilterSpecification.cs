using Adis.Dal.Interfaces;
using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
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
            Status? status = null,
            DateOnly? targetDate = null,
            DateOnly? startDateFrom = null,
            DateOnly? startDateTo = null)
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
        }
    }
}
