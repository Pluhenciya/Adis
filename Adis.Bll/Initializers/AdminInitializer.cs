using Adis.Bll.Configurations;
using Adis.Bll.Interfaces;
using Adis.Dal.Interfaces;
using Adis.Dm;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Initializers
{
    /// <inheritdoc cref="IAdminInitializer"/>
    public class AdminInitializer : IAdminInitializer
    {
        private readonly UserManager<User> _userManager;
        private readonly AdminSettings _adminSettings;

        public AdminInitializer(
            UserManager<User> userManager,
            IOptions<AdminSettings> adminSettings)
        {
            _userManager = userManager;
            _adminSettings = adminSettings.Value;
        }

        public async Task InitializeAsync()
        {

            var admins = await _userManager.GetUsersInRoleAsync("Admin");

            if (admins.Count() > 0)
                return;

            var admin = new User
            {
                Email = _adminSettings.Email,
                UserName = _adminSettings.Email
            };

            await _userManager.CreateAsync(admin, _adminSettings.Password);
        }
    }
}
