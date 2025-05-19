using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dm
{
    public class WorkObjectSection
    {
        public int IdWorkObjectSection { get; set; }

        public string Name { get; set; } = null!;

        public virtual IEnumerable<ExecutionTask> ExecutionTasks { get; set; } = null!;
    }
}
