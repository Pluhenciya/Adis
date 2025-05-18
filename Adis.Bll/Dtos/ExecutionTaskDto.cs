using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Dtos
{
    public class ExecutionTaskDto
    {
        public int IdExecutionTask { get; set; }

        public string Name { get; set; } = null!;

        public bool IsCompleted { get; set; }

        public WorkObjectSectionDto WorkObjectSection { get; set; } = null!;
    }
}
