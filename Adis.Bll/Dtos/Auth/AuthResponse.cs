using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Dtos.Auth
{
    public class AuthResponse
    {
        public string AccessToken { get; set; } = null!;
        public int ExpiresIn { get; set; }
        public string TokenType { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}
