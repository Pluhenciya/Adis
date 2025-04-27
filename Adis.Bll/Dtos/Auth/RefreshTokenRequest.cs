using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Dtos.Auth
{
    /// <summary>
    /// DTO запроса для обновления токена доступа
    /// </summary>
    public class RefreshTokenRequest
    {
        /// <summary>
        /// Старый токен доступа
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Токен доступа обязателен")]
        public string AccessToken { get; set; } = null!;

        /// <summary>
        /// Токен обновления
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Токен обновления обязателен")]
        public string RefreshToken { get; set; } = null!;
    }
}
