using Adis.Dal.Data;
using Adis.Dal.Interfaces;
using Adis.Dal.Specifications;
using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dal.Repositories
{
    public class RefreshTokenRepository : EFGenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<RefreshToken?> GetRefreshTokenByIdUserWithTokenAsync(string token, int idUser)
        {
            return (await GetAsync(new RefreshTokenSpecification(token, idUser))).FirstOrDefault();
        }
    }
}
