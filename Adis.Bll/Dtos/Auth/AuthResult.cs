using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Dtos.Auth
{
    public class AuthResult
    {
        public bool Success { get; set; }

        public string Token { get; set; } = null!;

        public string RefreshToken { get; set; } = null!;

        public IEnumerable<string> Errors { get; set; } = null!;

        public int ExpiresIn { get; set; }
    }
}
