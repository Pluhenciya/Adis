using Adis.Bll.Dtos.Task;
using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Dtos.Project
{
    /// <summary>
    /// DTO для проекта с его задачами
    /// </summary>
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

        /// <summary>
        /// ФИО ответственного лица
        /// </summary>
        public string ResponsiblePerson { get; set; } = null!;

        /// <summary>
        /// Прогресс проетирования/выполнения проекта
        /// </summary>
        public int Progress { get; set; }

        /// <summary>
        /// Объект выполнения работ
        /// </summary>
        public WorkObjectDto WorkObject { get; set; } = null!;

        /// <summary>
        /// Наименование подрядчика
        /// </summary>
        public string? ContractorName { get; set; } = null!;

        /// <summary>
        /// Дата начала выполнения работ
        /// </summary>
        public DateOnly? StartExecutionDate { get; set; }

        /// <summary>
        /// Дата окончания выполнения работ
        /// </summary>
        public DateOnly? EndExecutionDate { get; set; }

        /// <summary>
        /// Задачи проекта
        /// </summary>
        public IEnumerable<TaskDto> Tasks { get; set; } = null!;
    }
}
