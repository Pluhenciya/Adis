using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dm
{
    public class User
    {
        public int IdUser { get; set; }

        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public Role Role { get; set; }

        public string? FullName { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
