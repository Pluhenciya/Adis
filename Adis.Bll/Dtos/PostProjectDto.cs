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
        public string Name { get; set; } = null!;

        /// <summary>
        /// Начало проектирования
        /// </summary>
        public DateOnly StartDate { get; set; }

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

        public int IdLocation { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Место работ обязательно")]
        public string NameWorkObject { get; set; } = null!;

        [Required(ErrorMessage = "Локация обязательно")]
        public LocationDto Location { get; set; } = null!;
    }
}
