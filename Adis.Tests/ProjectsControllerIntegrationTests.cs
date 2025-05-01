using System.Net;
using System.Net.Http.Json;
using Adis.Bll.Dtos;
using Adis.Bll.Dtos.Auth;
using Adis.Dal.Data;
using Adis.Dm;
using Adis.Tests.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Adis.Tests
{
    public class ProjectsControllerIntegrationTests
        : IClassFixture<CustomWebApplicationFactory>, IDisposable
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;
        private readonly IServiceScope _scope;
        private readonly UserManager<User> _userManager;
        private string _testUserEmail;
        private string _testUserPassword;

        public ProjectsControllerIntegrationTests(CustomWebApplicationFactory factory)
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
            // Создаем тестового пользователя с подтвержденным email
            var user = new User
            {
                UserName = _testUserEmail,
                Email = _testUserEmail,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, _testUserPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
                throw new Exception($"User creation failed: {errors}");
            }
        }

        private async Task<string> GetAuthTokenAsync()
        {
            // Логинимся с созданными учетными данными
            var loginRequest = new LoginRequest
            {
                Email = _testUserEmail,
                Password = _testUserPassword
            };

            var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
            response.EnsureSuccessStatusCode();

            return (await response.Content.ReadFromJsonAsync<AuthResponse>())!.AccessToken;
        }

        public void Dispose()
        {
            _scope.Dispose();
            _client.Dispose();
        }

        [Fact]
        public async Task AddProject_ValidData_ReturnsCreatedProject()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

            var project = new ProjectDto
            {
                Name = "Test Project",
                Budget = 100000,
                StartDate = new DateOnly(2024, 1, 1),
                EndDate = new DateOnly(2024, 12, 31),
                Status = Status.Draft,
                IdUser = 1 // Должен соответствовать ID созданного пользователя
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/projects", project);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<ProjectDto>();
            Assert.Equal(project.Name, result!.Name);
        }

        [Fact]
        public async Task AddProject_Unauthorized_ReturnsUnauthorized()
        {
            // Arrange
            var project = new ProjectDto
            {
                Name = "Test Project",
                Budget = 100000,
                StartDate = new DateOnly(2024, 1, 1),
                EndDate = new DateOnly(2024, 12, 31),
                Status = Status.Draft,
                IdUser = 1
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/projects", project);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetProjects_WithFilters_ReturnsFilteredResults()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            // Add test projects
            var projects = new List<ProjectDto>
            {
                new()
                {
                    Name = "Filtered Project",
                    Budget = 500000,
                    StartDate = new DateOnly(2024, 1, 1),
                    EndDate = new DateOnly(2024, 3, 31),
                    Status = Status.Draft,
                    IdUser = 1
                },
                new()
                {
                    Name = "Other Project",
                    Budget = 1500000,
                    StartDate = new DateOnly(2024, 6, 1),
                    EndDate = new DateOnly(2024, 12, 31),
                    Status = Status.InProgress,
                    IdUser = 1
                }
            };


            foreach (var project in projects)
            {
                await _client.PostAsJsonAsync("/api/projects", project);
            }

            // Act
            var response = await _client.GetAsync("/api/projects?status=draft&startDateFrom=2024-01-01&startDateTo=2024-03-01");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ProjectsResponseDto>();
            Assert.Single(result.Projects);
            Assert.Equal("Filtered Project", result.Projects.First().Name);
        }
    }
}