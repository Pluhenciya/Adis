using Adis.Dm;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public int Id { get; set; }

        /// <summary>
        /// Почта пользователя
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Почта обязательна")]
        [EmailAddress]
        public string Email { get; set; } = null!;

        /// <summary>
        /// Пароль пользователя
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Пароль обязателен")]
        public string Password { get; set; } = null!;

        /// <summary>
        /// Роль пользователя
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Роль обязательна")]
        public string Role { get; set; } = null!;

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
