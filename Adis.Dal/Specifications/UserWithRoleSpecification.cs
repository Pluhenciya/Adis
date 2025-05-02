using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dal.Specifications
{
    /// <summary>
    /// Спецификация для получения пользователей с ролью
    /// </summary>
    public class UserWithRoleSpecification : Specification<User>
    {
        public UserWithRoleSpecification() 
        {
            AddInclude(u => u.Roles);
        }
    }
}
