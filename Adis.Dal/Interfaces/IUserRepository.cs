using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dal.Interfaces
{
    /// <summary>
    /// Позоляет управлять данными пользователей
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        public Task<IEnumerable<User>> GetUsersWithRoleAsync();
    }
}
