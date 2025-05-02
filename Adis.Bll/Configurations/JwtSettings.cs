using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Configurations
{
    /// <summary>
    /// Конфигурация JWT авторизации
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// Cекретный ключ для подписи JWT токена
        /// </summary>
        public string Key { get; set; } = null!;

        /// <summary>
        /// Сторона, генерирующая токен 
        /// </summary>
        public string Issuer { get; set; } = null!;

        /// <summary>
        /// Идентификатор сервиса, который должен принимать токен
        /// </summary>
        public string Audience { get; set; } = null!;

        /// <summary>
        /// Время жизни токена доступа
        /// </summary>
        public TimeSpan TokenLifetime { get; set; } = TimeSpan.FromMinutes(15);

        /// <summary>
        /// Время жизни токена обновления
        /// </summary>
        public TimeSpan RefreshTokenLifetime { get; set; } = TimeSpan.FromDays(7);
    }
}
