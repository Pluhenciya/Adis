using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Dtos
{
    /// <summary>
    /// DTO для пользователей
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public int IdUser { get; set; }

        /// <summary>
        /// Почта пользователя
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Хэш пароля пользователя
        /// </summary>
        public string PasswordHash { get; set; } = null!;

        /// <summary>
        /// Роль пользователя
        /// </summary>
        public Role Role { get; set; }

        /// <summary>
        /// ФИО пользователя
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// Время создателя проекта
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
