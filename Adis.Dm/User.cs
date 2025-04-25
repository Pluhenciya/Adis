using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dm
{
    /// <summary>
    /// Основная модель пользователя
    /// </summary>
    public class User
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

        /// <summary>
        /// Проекты пользователя
        /// </summary>
        public virtual IEnumerable<Project> Projects { get; set; } = null!;
    }
}
