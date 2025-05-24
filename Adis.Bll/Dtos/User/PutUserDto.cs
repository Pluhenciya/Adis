using Adis.Dm;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Dtos.User
{
    public class PutUserDto
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
        public string? Password { get; set; }

        /// <summary>
        /// Роль пользователя
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Роль обязательна")]
        public Role? Role { get; set; }

        /// <summary>
        /// ФИО пользователя
        /// </summary>
        public string? FullName { get; set; }
    }
}
