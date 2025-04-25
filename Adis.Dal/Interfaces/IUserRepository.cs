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
        /// <summary>
        /// Возвращает пользователя по почте
        /// </summary>
        /// <param name="email">Почта возвращаемоего пользователя</param>
        /// <returns>Пользователь с указанной почтой</returns>
        public Task<User?> GetUserByEmailAsync(string email);
    }
}
