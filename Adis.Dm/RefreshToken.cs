using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dm
{
    public class RefreshToken
    {
        public int IdRefreshToken { get; set; }
        public string Token { get; set; } = null!;
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
        public string CreatedByIp { get; set; } = null!;
        public DateTime? Revoked { get; set; }
        public string? RevokedByIp { get; set; } = null!;
        public string? ReplacedByToken { get; set; } = null!;
        public int IdUser { get; set; }
        public User User { get; set; } = null!;
    }

}
