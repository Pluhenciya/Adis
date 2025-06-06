using Adis.Dm;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Dtos.Task
{
    public class PostTaskDto
    {
        /// <summary>
        /// Идентификатор задачи
        /// </summary>
        public int IdTask { get; set; }

        /// <summary>
        /// Наименование задачи
        /// </summary>
        [StringLength(255)]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Описание задачи
        /// </summary>
        public string Description { get; set; } = null!;

        public IEnumerable<int> IdPerformers { get; set; } = null!;

        public IEnumerable<int> IdCheckers { get; set; } = null!;

        public DateOnly PlannedEndDate { get; set; }

        public int IdProject { get; set; }

        public Status Status { get; set; }
    }
}
