using Adis.Dal.Data;
using Adis.Dal.Interfaces;
using Adis.Dal.Specifications;
using Adis.Dm;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<User> _userManager;

        public UserRepository(
            UserManager<User> userManager,
            RoleManager<AppRole> roleManager,
            AppDbContext dbContext) : base(dbContext)
        {
            _userManager = userManager;
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task CreateUserAsync(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new ArgumentException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task AddToRoleAsync(User user, string role)
        {
            var result = await _userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded)
                throw new ArgumentException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task RemoveFromRoleAsync(User user, string role)
        {
            var result = await _userManager.RemoveFromRoleAsync(user, role);
            if (!result.Succeeded)
                throw new ArgumentException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task UpdateUserAsync(User user, string? newPassword = null)
        {
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new ArgumentException(string.Join(", ", result.Errors.Select(e => e.Description)));

            if (newPassword != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, newPassword);
                if (!passwordResult.Succeeded)
                    throw new ArgumentException(string.Join(", ", passwordResult.Errors.Select(e => e.Description)));
            }
        }

        public async Task DeleteUserAsync(User user)
        {
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                throw new ArgumentException(string.Join(", ", result.Errors.Select(e => e.Description)));
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
        