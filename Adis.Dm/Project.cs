using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dm
{
    public class Project
    {
        public int IdProduct { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public double Budget { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public Status Status { get; set; }
    }
}
