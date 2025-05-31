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

        public Status Status { get; set; }

        public DateOnly PlannedEndDate { get; set; }

        public DateOnly? ActualEndDate { get; set; }

        public string? TextResult { get; set; } = null!;

        /// <summary>
        /// Проект с этой задачей
        /// </summary>
        public virtual Project Project { get; set; } = null!;

        public virtual IEnumerable<User> Performers { get; set; } = null!;

        public virtual IEnumerable<User> Checkers { get; set; } = null!;

        public virtual IEnumerable<Document> Documents { get; set; } = null!;

        public virtual IEnumerable<Comment> Comments { get; set; } = null!;
    }
}
