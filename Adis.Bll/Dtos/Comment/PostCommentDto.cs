using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Dtos.Comment
{
    /// <summary>
    /// DTO для получения комментария
    /// </summary>
    public class PostCommentDto
    {
        /// <summary>
        /// Идентификатор задачи, которой принадлежит комментарий
        /// </summary>
        public int IdTask { get; set; }

        /// <summary>
        /// Текст комментария
        /// </summary>
        public string Text { get; set; } = null!;
    }
}
