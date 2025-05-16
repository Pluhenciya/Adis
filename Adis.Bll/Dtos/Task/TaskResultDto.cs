using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Dtos.Task
{
    public class TaskResultDto
    {
        [Required]
        public string Result { get; set; } = null!;
    }
}
