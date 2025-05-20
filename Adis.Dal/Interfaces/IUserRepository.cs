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
        /// Возвращает пользователя с ролью
        /// </summary>
        /// <returns>Пользователь с ролью</returns>
        public Task<IEnumerable<User>> GetUsersWithRoleAsync();

        public Task<User?> GetUserWithRoleByIdAsync(int id);

        public Task<IEnumerable<User>> GetUsersByPartialFullNameWithRoleAsync(string partialFullName, Role role);

        public Task<User?> FindByEmailAsync(string email);
        public Task CreateUserAsync(User user, string password);
        public Task AddToRoleAsync(User user, string role);
        public Task RemoveFromRoleAsync(User user, string role);
        public Task UpdateUserAsync(User user, string? newPassword = null);
        public Task DeleteUserAsync(User user);
    }
}
