using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dal.Specifications
{
    public class UserWithRoleSpecification : Specification<User>
    {
        public UserWithRoleSpecification() 
        {
            AddInclude(u => u.Roles);
        }
    }
}
