using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dm
{
    public class Document
    {
        public int IdDocument { get; set; }

        public string FileName { get; set; } = null!;

        public int? IdUser { get; set; }

        public DocumentType DocumentType { get; set; }

        public virtual User? User { get; set; }

        public virtual IEnumerable<ProjectTask> Tasks { get; set; } = null!;
    }
}
