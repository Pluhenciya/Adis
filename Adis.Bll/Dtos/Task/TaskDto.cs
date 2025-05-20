using Adis.Bll.Dtos.User;
using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Dtos.Task
{
    public class TaskDto
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

        public Status Status { get; set; }

        public IEnumerable<UserDto> Performers { get; set; } = null!;

        public IEnumerable<UserDto> Checkers { get; set; } = null!;
    }
}
