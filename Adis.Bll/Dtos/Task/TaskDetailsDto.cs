using Adis.Bll.Dtos.Comment;
using Adis.Bll.Dtos.User;
using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Dtos.Task
{
    public class TaskDetailsDto
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

        public string? TextResult { get; set; } = null!;

        public DateOnly PlannedEndDate { get; set; }
        public DateOnly? ActualEndDate { get; set; }

        public IEnumerable<UserDto> Performers { get; set; } = null!;

        public IEnumerable<UserDto> Checkers { get; set; } = null!;

        public IEnumerable<DocumentDto> Documents { get; set; } = null!;

        public IEnumerable<CommentDto> Comments { get; set; } = null!;
    }
}
