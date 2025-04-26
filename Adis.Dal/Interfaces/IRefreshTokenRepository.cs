using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dal.Interfaces
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        public Task<RefreshToken?> GetRefreshTokenByIdUserWithTokenAsync(string token, int idUser);
    }
}
