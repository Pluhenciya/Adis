using Adis.Dm;
using Duende.IdentityModel;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Identity
{
    public class CustomProfileService : IProfileService
    {
        private readonly UserManager<User> _userManager;

        public CustomProfileService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await _userManager.GetUserAsync(context.Subject);
            var roles = await _userManager.GetRolesAsync(user);

            context.IssuedClaims.AddRange(new[]
            {
            new Claim(JwtClaimTypes.Subject, user.Id.ToString()),
            new Claim(JwtClaimTypes.Email, user.Email),
            new Claim(JwtClaimTypes.Name, user.FullName ?? string.Empty)
        });

            context.IssuedClaims.AddRange(roles.Select(role =>
                new Claim(JwtClaimTypes.Role, role)));
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var user = await _userManager.GetUserAsync(context.Subject);
            context.IsActive = user != null;
        }
    }
}
