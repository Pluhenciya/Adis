using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Dtos.Project
{
    public class GetProjectWithTasksDto
    {
        /// <summary>
        /// Идентификатор проекта
        /// </summary>
        public int IdProject { get; set; }

        /// <summary>
        /// Название проекта
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Начало проектирования
        /// </summary>
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// Конец проектирования
        /// </summary>
        public DateOnly EndDate { get; set; }

        /// <summary>
        /// Статус проекта
        /// </summary>
        public ProjectStatus Status { get; set; } = ProjectStatus.Designing;

        /// <summary>
        /// Идентификатор пользователя, который создал проект
        /// </summary>
        public int IdUser { get; set; }

        public string ResponsiblePerson { get; set; } = null!;

        public int Progress { get; set; }

        public WorkObjectDto WorkObject { get; set; } = null!;

        public string? ContractorName { get; set; } = null!;

        public DateOnly? StartExecutionDate { get; set; }

        public DateOnly? EndExecutionDate { get; set; }

        public IEnumerable<TaskDto> Tasks { get; set; } = null!;
    }
}
