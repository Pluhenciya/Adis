using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Adis.Dm
{
    /// <summary>
    /// Роль
    /// </summary>
    public class AppRole : IdentityRole<int>
    {
        /// <summary>
        /// Пользователи с этой ролью
        /// </summary>
        public virtual IEnumerable<User> Users { get; set; } = null!;
    }
}
