using Adis.Dm;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Dtos
{
    public class WorkObjectDto
    {
        public int IdLocation { get; set; }
        public Geometry Geometry { get; set; } = null!;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Место работ обязательно")]
        [StringLength(255)]
        public string Name { get; set; } = null!;
    }
}
