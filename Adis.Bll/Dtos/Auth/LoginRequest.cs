using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Dtos.Auth
{
    /// <summary>
    /// DTO запроса авторизации с данными пользователя
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Почта
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Почта обязательна")]
        [EmailAddress]
        public string Email { get; set; } = null!;

        /// <summary>
        /// Пароль
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Пароль обязательен")]
        public string Password { get; set; } = null!;
    }
}
