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

        public async Task<IEnumerable<User>> GetUsersWithRoleAsync()
        {
            UserWithRoleSpecification specification = new UserWithRoleSpecification();
            return await GetAsync(specification);
        }

        public async Task<User?> GetUserWithRoleByIdAsync(int id)
        {
            UserWithRoleSpecification specification = new UserWithRoleSpecification(id);
            return (await GetAsync(specification)).FirstOrDefault();
        }

        public async Task<IEnumerable<User>> GetUsersByPartialFullNameWithRoleAsync(string partialFullName, Role role)
        {
            UserByPartialFullNameSpecificationWithRole spec = new(partialFullName, role);
            return await GetAsync(spec);
        }
    }
}
        