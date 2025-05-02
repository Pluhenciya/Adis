using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Dtos.Auth
{
    /// <summary>
    /// Dto ответа для успешной авторизации
    /// </summary>
    public class AuthResponse
    {
        /// <summary>
        /// Токен доступа
        /// </summary>
        public string AccessToken { get; set; } = null!;

        /// <summary>
        /// Время жизни токена доступа
        /// </summary>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Тип токена
        /// </summary>
        public string TokenType { get; set; } = null!;

        /// <summary>
        /// Токен обновления
        /// </summary>
        public string RefreshToken { get; set; } = null!;
    }
}
