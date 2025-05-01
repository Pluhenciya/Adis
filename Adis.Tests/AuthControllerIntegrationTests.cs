using Adis.Bll.Dtos.Auth;
using Adis.Dal.Data;
using Adis.Dm;
using Adis.Tests.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace Adis.Tests
{
    public class AuthControllerIntegrationTests
        : IClassFixture<CustomWebApplicationFactory>, IDisposable
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;
        private readonly IServiceScope _scope;
        private readonly UserManager<User> _userManager;
        private string _testUserEmail;
        private string _testUserPassword;

        public AuthControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _scope = _factory.Services.CreateScope();
            _userManager = _scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            // Генерация уникальных данных для каждого теста
            var guid = Guid.NewGuid().ToString("N");
            _testUserEmail = $"user-{guid}@example.com";
            _testUserPassword = $"Password-{guid}!";

            InitializeTestUserAsync().Wait();
        }

        private async Task InitializeTestUserAsync()
        {
            var user = new User
            {
                UserName = _testUserEmail,
                Email = _testUserEmail,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, _testUserPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join("\n", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
                throw new Exception($"User creation failed:\n{errors}");
            }
        }

        public void Dispose()
        {
            _scope.Dispose();
            _client.Dispose();
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsTokens()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = _testUserEmail,
                Password = _testUserPassword
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", request);

            // Assert
            response.EnsureSuccessStatusCode();
            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
            Assert.NotNull(authResponse?.AccessToken);
            Assert.NotNull(authResponse?.RefreshToken);
        }

        [Fact]
        public async Task RefreshToken_ValidTokens_ReturnsNewTokens()
        {
            // Arrange
            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login",
                new LoginRequest
                {
                    Email = _testUserEmail,
                    Password = _testUserPassword
                });

            var authResponse = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();

            // Act
            var refreshResponse = await _client.PostAsJsonAsync("/api/auth/refresh-token",
                new RefreshTokenRequest
                {
                    AccessToken = authResponse!.AccessToken,
                    RefreshToken = authResponse.RefreshToken
                });

            // Assert
            refreshResponse.EnsureSuccessStatusCode();
            var newAuthResponse = await refreshResponse.Content.ReadFromJsonAsync<AuthResponse>();
            Assert.NotEqual(authResponse.AccessToken, newAuthResponse!.AccessToken);
            Assert.NotNull(newAuthResponse.RefreshToken);
        }
    }
}