using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Dtos
{
    /// <summary>
    /// DTO для проектов
    /// </summary>
    public class ProjectDto
    {
        /// <summary>
        /// Идентификатор проекта
        /// </summary>
        public int IdProduct { get; set; }

        /// <summary>
        /// Название проекта
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Описание проекта
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Бюджет проекта
        /// </summary>
        public double Budget { get; set; }

        /// <summary>
        /// Начало выполнения проекта
        /// </summary>
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// Конец выполнения проекта
        /// </summary>
        public DateOnly EndDate { get; set; }

        /// <summary>
        /// Статус проекта
        /// </summary>
        public Status Status { get; set; }
    }
}
