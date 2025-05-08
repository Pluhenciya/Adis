using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dm
{
    public class Location
    {
        public int IdLocation { get; set; }
        public Geometry Geometry { get; set; } = null!;
        public IEnumerable<Project> Projects { get; set; } = null!;
    }
}
