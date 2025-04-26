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
    /// <inheritdoc cref="IUserRepository"/>
    public class UserRepository : EFGenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
        