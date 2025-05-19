using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dm
{
    public class ExecutionTask
    {
        public int IdExecutionTask { get; set; }

        public string Name { get; set; } = null!;

        public bool IsCompleted { get; set; }

        public int IdProject { get; set; }

        public virtual Project Project { get; set; } = null!;

        public int IdWorkObjectSection { get; set; }

        public virtual WorkObjectSection WorkObjectSection { get; set; } = null!;
    }
}
