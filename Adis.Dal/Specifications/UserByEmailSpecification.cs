using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dal.Specifications
{
    /// <summary>
    /// Спецификация для фильтрации пользователей по почте
    /// </summary>
    public class UserByEmailSpecification : Specification<User>
    {
        public UserByEmailSpecification(string email) 
        {
            ApplyCriteria(u => u.Email == email);
        }
    }
}
