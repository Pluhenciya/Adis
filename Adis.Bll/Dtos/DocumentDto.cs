using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Dtos
{
    public class DocumentDto
    {
        public int IdDocument { get; set; }

        public string FileName { get; set; } = null!;

        public int? IdUser { get; set; }

        public DocumentType DocumentType { get; set; }
    }
}
