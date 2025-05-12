using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dm
{
    public class Comment
    {
        public int IdComment { get; set; }

        public string Text { get; set; } = null!;

        public int IdSender { get; set; }

        public virtual User Sender { get; set; } = null!;

        public int IdTask { get; set; }

        public virtual ProjectTask  Task { get; set; } = null!;
    }
}
