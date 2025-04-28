using Adis.Dm;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Required(AllowEmptyStrings = false, ErrorMessage = "Наименование обязательно")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Описание проекта
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Бюджет проекта
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Бюджет обязателен")]
        [Range(0.01, 1000000000000, ErrorMessage = "Допустимый бюджет от 0.01 до 1 000 000 000 000")]
        public double Budget { get; set; }

        /// <summary>
        /// Начало выполнения проекта
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Дата начала обязательна")]
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// Конец выполнения проекта
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Дата конца обязательна")]
        public DateOnly EndDate { get; set; }

        /// <summary>
        /// Статус проекта
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Статус проекта обязателен")]
        public Status Status { get; set; }

        /// <summary>
        /// Время создания
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        [Required(ErrorMessage = "Идентификатор пользователя обязателен")]
        public int IdUser { get; set; }
    }
}
