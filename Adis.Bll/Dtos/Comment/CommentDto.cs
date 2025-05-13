using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Dtos.Comment
{
    public class CommentDto
    {
        public int IdComment { get; set; }

        public string Text { get; set; } = null!;

        public int IdSender { get; set; }

        public string FullNameSender { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}
