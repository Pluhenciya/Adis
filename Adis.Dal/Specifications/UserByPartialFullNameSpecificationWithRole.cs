using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dal.Specifications
{
    public class UserByPartialFullNameSpecificationWithRole : Specification<User>
    {
        public UserByPartialFullNameSpecificationWithRole(string partialFullName, Role role)
        {
            ApplyCriteria(u => u.FullName!.Contains(partialFullName) && u.Roles.Any(r => r.NormalizedName == role.ToString().ToUpper()));

            AddInclude(u => u.Roles);
        }
    }
}
