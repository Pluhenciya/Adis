using Adis.Bll.Dtos.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Dtos
{
    /// <summary>
    /// DTO со списком проектов и данными для пагинации
    /// </summary>
    public class ProjectsResponseDto
    {
        /// <summary>
        /// Список проектов
        /// </summary>
        public IEnumerable<GetProjectDto> Projects { get; set; } = null!;

        /// <summary>
        /// Общее количество проектов 
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Номер страницы
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Размер страницы
        /// </summary>
        public int PageSize { get; set; }
    }
}
