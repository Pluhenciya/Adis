using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dm
{
    /// <summary>
    /// Основная модель проекта
    /// </summary>
    public class Project
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
        public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        /// <summary>
        /// Конец проектирования
        /// </summary>
        public DateOnly PlannedEndDate { get; set; }

        public DateOnly? ActualEndDate { get; set; }

        /// <summary>
        /// Статус проекта
        /// </summary>
        public ProjectStatus Status { get; set; }

        public DateOnly? StartExecutionDate { get; set; }

        public DateOnly? PlannedEndExecutionDate { get; set; }

        public DateOnly? ActualEndExecutionDate { get; set; } 

        public bool IsDeleted { get; set; }

        /// <summary>
        /// Идентификатор пользователя создавшего проект
        /// </summary>
        public int IdUser { get; set; }

        public int IdWorkObject { get; set; }

        public int? IdContractor { get; set; }

        public virtual Contractor? Contractor { get; set; } = null!;

        public virtual WorkObject WorkObject { get; set; } = null!;

        /// <summary>
        /// Пользователь создавший проект
        /// </summary>
        public virtual User User { get; set; } = null!;

        /// <summary>
        /// Задачи проекта
        /// </summary>
        public virtual IEnumerable<ProjectTask> Tasks { get; set; } = null!;

        public virtual IEnumerable<ExecutionTask> ExecutionTasks { get; set; } = null!;
    }
}
