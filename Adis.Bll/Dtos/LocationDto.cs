using Adis.Dm;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Dtos
{
    public class LocationDto
    {
        public int IdLocation { get; set; }
        public Geometry Geometry { get; set; } = null!;
    }
}
