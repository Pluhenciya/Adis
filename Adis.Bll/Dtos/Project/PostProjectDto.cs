using Adis.Dm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Dtos.Project
{
    /// <summary>
    /// DTO для проектов
    /// </summary>
    public class PostProjectDto
    {
        /// <summary>
        /// Идентификатор проекта
        /// </summary>
        public int IdProject { get; set; }

        /// <summary>
        /// Название проекта
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Наименование обязательно")]
        [StringLength(255)]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Начало проектирования
        /// </summary>
        public DateOnly? StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        /// <summary>
        /// Конец проектирования
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Дата конца обязательна")]
        public DateOnly EndDate { get; set; }

        /// <summary>
        /// Статус проекта
        /// </summary>
        public ProjectStatus Status { get; set; } = ProjectStatus.Designing;

        /// <summary>
        /// Идентификатор пользователя, который создал проект
        /// </summary>
        public int IdUser { get; set; }

        public int IdWorkObject { get; set; }

        public WorkObjectDto WorkObject { get; set; } = null!;

        [StringLength(255)]
        public string? ContractorName { get; set; } = null!;

        public int? IdContractor { get; set; }

        public DateOnly? StartExecutionDate { get; set; }

        public DateOnly? EndExecutionDate { get; set; }
    }
}
