using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Configurations
{
    public class JwtSettings
    {
        public string Key { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public TimeSpan TokenLifetime { get; set; } = TimeSpan.FromMinutes(15);
        public TimeSpan RefreshTokenLifetime { get; set; } = TimeSpan.FromDays(7);
    }
}
