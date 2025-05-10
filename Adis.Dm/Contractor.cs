using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dm
{
    public class Contractor
    {
        public int IdContractor { get; set; }

        public string Name { get; set; } = null!;

        public virtual IEnumerable<Project> Projects { get; set; } = null!;
    }
}
