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
        public DateOnly EndDate { get; set; }

        /// <summary>
        /// Статус проекта
        /// </summary>
        public ProjectStatus Status { get; set; }

        /// <summary>
        /// Наименование объекта, на котором проводятся работы
        /// </summary>
        public string NameWorkObject { get; set; } = null!;

        public DateOnly? StartExecutionDate { get; set; }

        public DateOnly? EndExecutionDate { get; set; }

        /// <summary>
        /// Идентификатор пользователя создавшего проект
        /// </summary>
        public int IdUser { get; set; }

        public int IdLocation { get; set; }

        public int? IdConstractor { get; set; }

        public virtual Constractor Constractor { get; set; } = null!;

        public virtual Location Location { get; set; } = null!;

        /// <summary>
        /// Пользователь создавший проект
        /// </summary>
        public virtual User User { get; set; } = null!;

        /// <summary>
        /// Задачи проекта
        /// </summary>
        public virtual IEnumerable<ProjectTask> Tasks { get; set; } = null!;
    }
}
