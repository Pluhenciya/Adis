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

        /// <summary>
        /// Идентификатор пользователя создавшего проект
        /// </summary>
        public int IdUser { get; set; }

        /// <summary>
        /// Пользователь создавший проект
        /// </summary>
        public virtual User User { get; set; } = null!;
    }
}
