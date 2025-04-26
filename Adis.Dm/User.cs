using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dm
{
    public class User : IdentityUser<int> 
    {
        public virtual IEnumerable<Role> Roles { get; set; } = null!;

        /// <summary>
        /// ФИО пользователя
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// Время создания пользователя
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Проекты пользователя
        /// </summary>
        public virtual IEnumerable<Project> Projects { get; set; } = null!;

        public virtual IEnumerable<RefreshToken> RefreshTokens { get; set; }= null!;
    }
}
