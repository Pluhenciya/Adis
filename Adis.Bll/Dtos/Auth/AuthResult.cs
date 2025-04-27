using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Dtos.Auth
{
    /// <summary>
    /// Результат авторизации
    /// </summary>
    public class AuthResult
    {
        /// <summary>
        /// Удачно ли
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Токен доступа
        /// </summary>
        public string Token { get; set; } = null!;

        /// <summary>
        /// Токен обновления
        /// </summary>
        public string RefreshToken { get; set; } = null!;

        /// <summary>
        /// Список ошибок (если неудачно)
        /// </summary>
        public IEnumerable<string> Errors { get; set; } = null!;

        /// <summary>
        /// Время жизни токена доступа
        /// </summary>
        public int ExpiresIn { get; set; }
    }
}
