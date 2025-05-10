using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dal.Data
{
    public class InitialData
    {
        public static List<AppRole> RolesList { get; set; } = new List<AppRole>()
        {
           new AppRole { Id = 1, Name = Role.Admin.ToString(), NormalizedName = Role.Admin.ToString().ToUpper()},
           new AppRole { Id = 2, Name = Role.Projecter.ToString(), NormalizedName = Role.Projecter.ToString().ToUpper() },
           new AppRole { Id = 3, Name = Role.ProjectManager.ToString(), NormalizedName = Role.ProjectManager.ToString().ToUpper() },
        };
    }
}
