using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Dtos.Comment
{
    /// <summary>
    /// DTO для возврата комментария
    /// </summary>
    public class CommentDto
    {
        /// <summary>
        /// Идентификатор комментария
        /// </summary>
        public int IdComment { get; set; }

        /// <summary>
        /// Текст комментария
        /// </summary>
        public string Text { get; set; } = null!;

        /// <summary>
        /// Идентификатор отправителя
        /// </summary>
        public int IdSender { get; set; }

        /// <summary>
        /// ФИО отправителя
        /// </summary>
        public string FullNameSender { get; set; } = null!;

        /// <summary>
        /// Дата создания комментария
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
