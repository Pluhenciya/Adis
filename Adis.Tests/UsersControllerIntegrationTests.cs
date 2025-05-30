﻿using Adis.Bll.Dtos.Auth;
using Adis.Bll.Dtos.User;
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
    /// <summary>
    /// Позволяет тестировать работу с пользователями в API
    /// </summary>
    public class UsersControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;
        private readonly IServiceScope _scope;
        private readonly AppDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public UsersControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _scope = _factory.Services.CreateScope();
            _dbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
            _userManager = _scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            _roleManager = _scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

            InitializeAsync().Wait();
        }

        /// <summary>
        /// Создает пользователей и роли для тестов
        /// </summary>
        private async Task InitializeAsync()
        {
            // Создаем роли
            await CreateRoleIfNotExists(Role.Admin.ToString());
            await CreateRoleIfNotExists(Role.Projecter.ToString());

            // Создаем администратора
            var admin = new User
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
                EmailConfirmed = true
            };
            await CreateUserIfNotExists(admin, "AdminPassword123!", Role.Admin.ToString());

            // Создаем обычного пользователя
            var regularUser = new User
            {
                UserName = "user@example.com",
                Email = "user@example.com",
                EmailConfirmed = true
            };
            await CreateUserIfNotExists(regularUser, "UserPassword123!", Role.Projecter.ToString());
        }

        /// <summary>
        /// Создает роль если её не существует
        /// </summary>
        /// <param name="roleName">Имя роли</param>
        private async Task CreateRoleIfNotExists(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new AppRole { Name = roleName});
            }
        }

        /// <summary>
        /// Создает пользователя если его не существует
        /// </summary>
        /// <param name="user">Данные пользователя</param>
        /// <param name="password">Пароль пользователя</param>
        /// <param name="role">Роль пользователя</param>
        private async Task CreateUserIfNotExists(User user, string password, string role)
        {
            if (await _userManager.FindByEmailAsync(user.Email!) == null)
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

        /// <summary>
        /// Возвращает токен администратора
        /// </summary>
        /// <returns>Токен администратора</returns>
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

        /// <summary>
        /// Возвращает токен не администратора
        /// </summary>
        /// <returns>Токен не администратора</returns>
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

        /// <summary>
        /// Тестирует создание пользователя под администратором
        /// </summary>
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
                Role = Role.Projecter,
                FullName = "New User"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/users", newUser);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<UserDto>();
            Assert.Equal(newUser.Email, result!.Email);
            Assert.Equal(newUser.Role, result.Role);
        }

        /// <summary>
        /// Тестирует невозможность создания пользователя под не администратором
        /// </summary>
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
                Role = Role.Projecter
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/users", newUser);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}