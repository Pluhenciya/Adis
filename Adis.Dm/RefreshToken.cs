using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dm
{
    /// <summary>
    /// Основная модель токена обновления
    /// </summary>
    public class RefreshToken
    {
        /// <summary>
        /// Идентификатор токена
        /// </summary>
        public int IdRefreshToken { get; set; }

        /// <summary>
        /// Значение токена
        /// </summary>
        public string Token { get; set; } = null!;

        /// <summary>
        /// Время оканяания срока использования токена
        /// </summary>
        public DateTime Expires { get; set; }

        /// <summary>
        /// Время создания
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// С какого IP был создан
        /// </summary>
        public string CreatedByIp { get; set; } = null!;

        /// <summary>
        /// Время использования
        /// </summary>
        public DateTime? Revoked { get; set; }

        /// <summary>
        /// С какого IP был использован
        /// </summary>
        public string? RevokedByIp { get; set; } = null!;

        /// <summary>
        /// Следущий токен обновления
        /// </summary>
        public string? ReplacedByToken { get; set; } = null!;

        /// <summary>
        /// Идетификатор пользователя, которому он пренадлежит
        /// </summary>
        public int IdUser { get; set; }

        /// <summary>
        /// Пользователь, которому он пренадлежит
        /// </summary>
        public User User { get; set; } = null!;
    }

}
