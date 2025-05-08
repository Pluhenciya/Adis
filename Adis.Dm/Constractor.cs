using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dm
{
    public class Constractor
    {
        public int IdConstractor { get; set; }

        public string Name { get; set; } = null!;

        public virtual IEnumerable<Project> Projects { get; set; } = null!;
    }
}
