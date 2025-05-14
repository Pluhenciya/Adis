using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Dtos.Comment
{
    public class PostCommentDto
    {
        public int IdTask { get; set; }
        public string Text { get; set; } = null!;
    }
}
