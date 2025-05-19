using Adis.Bll.Dtos;
using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Interfaces
{
    /// <summary>
    /// Позволяет управлять данными пользователей
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Добавляет нового пользователя
        /// </summary>
        /// <param name="userDto">Данные нового пользователя</param>
        /// <returns>Созданный пользователь</returns>
        public Task<UserDto> AddUserAsync(UserDto userDto);

        /// <summary>
        /// Возвращает список всех пользователей
        /// </summary>
        /// <returns>Список всех пользователей</returns>
        public Task<IEnumerable<UserDto>> GetUsersAsync();

        public Task<UserDto?> GetUserByIdAsync(int id);

        public Task<IEnumerable<UserDto>> GetUsersByPartialFullNameWithRoleAsync(string partialFullName, string role);
    }
}
