using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Configurations
{
    /// <summary>
    /// Конфигурация первого администратора
    /// </summary>
    public class AdminSettings
    {
        /// <summary>
        /// Почта первого администратора
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Пароль первого администратора
        /// </summary>
        public string Password { get; set; } = null!;
    }
}
