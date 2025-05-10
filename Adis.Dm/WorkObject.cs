using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dm
{
    public class WorkObject
    {
        public int IdWorkObject { get; set; }

        /// <summary>
        /// Наименование объекта, на котором проводятся работы
        /// </summary>
        public string Name { get; set; } = null!;

        public Geometry Geometry { get; set; } = null!;
        public IEnumerable<Project> Projects { get; set; } = null!;
    }
}
