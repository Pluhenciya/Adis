using Adis.Bll.Dtos;
using Adis.Bll.Dtos.Auth;
using Adis.Dal.Data;
using Adis.Dm;
using Adis.Tests.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;

namespace Adis.Tests
{
    public class UsersControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;
        private readonly IServiceScope _scope;
        private readonly AppDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public UsersControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _scope = _factory.Services.CreateScope();
            _dbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
            _userManager = _scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            _roleManager = _scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

            InitializeAsync().Wait();
        }

        private async Task InitializeAsync()
        {
            // Создаем роли
            await CreateRoleIfNotExists("Admin");
            await CreateRoleIfNotExists("User");

            // Создаем администратора
            var admin = new User
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
                EmailConfirmed = true
            };
            await CreateUserIfNotExists(admin, "AdminPassword123!", "Admin");

            // Создаем обычного пользователя
            var regularUser = new User
            {
                UserName = "user@example.com",
                Email = "user@example.com",
                EmailConfirmed = true
            };
            await CreateUserIfNotExists(regularUser, "UserPassword123!", "User");
        }

        private async Task CreateRoleIfNotExists(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new Role { Name = roleName});
            }
        }

        private async Task CreateUserIfNotExists(User user, string password, string role)
        {
            if (await _userManager.FindByEmailAsync(user.Email) == null)
            {
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, role);
                }
            }
        }

        public void Dispose()
        {
            _scope.Dispose();
            _client.Dispose();
        }

        private async Task<string> GetAdminTokenAsync()
        {
            var loginRequest = new LoginRequest
            {
                Email = "admin@example.com",
                Password = "AdminPassword123!"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
            return (await response.Content.ReadFromJsonAsync<AuthResponse>())!.AccessToken;
        }

        private async Task<string> GetUserTokenAsync()
        {
            var loginRequest = new LoginRequest
            {
                Email = "user@example.com",
                Password = "UserPassword123!"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
            return (await response.Content.ReadFromJsonAsync<AuthResponse>())!.AccessToken;
        }

        [Fact]
        public async Task AddUser_AsAdmin_ReturnsCreatedUser()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

            var newUser = new UserDto
            {
                Email = "new.user@example.com",
                Password = "NewUserPassword123!",
                Role = "User",
                FullName = "New User"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/users", newUser);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<UserDto>();
            Assert.Equal(newUser.Email, result.Email);
            Assert.Equal(newUser.Role, result.Role);
        }

        [Fact]
        public async Task AddUser_AsNonAdmin_ReturnsForbidden()
        {
            // Arrange
            var token = await GetUserTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

            var newUser = new UserDto
            {
                Email = "another.user@example.com",
                Password = "AnotherPassword123!",
                Role = "Projecter"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/users", newUser);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}