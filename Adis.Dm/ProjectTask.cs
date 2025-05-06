using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dm
{
    /// <summary>
    /// Основная модель задачи
    /// </summary>
    public class ProjectTask
    {
        /// <summary>
        /// Идентификатор задачи
        /// </summary>
        public int IdTask { get; set; }

        /// <summary>
        /// Наименование задачи
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Описание задачи
        /// </summary>
        public string Description { get; set; } = null!;

        /// <summary>
        /// Идентификатор проекта с этой задачей
        /// </summary>
        public int IdProject { get; set; }

        /// <summary>
        /// Проект с этой задачей
        /// </summary>
        public virtual Project Project { get; set; } = null!;
    }
}
