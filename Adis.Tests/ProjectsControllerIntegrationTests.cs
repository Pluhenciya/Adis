using System.Net;
using System.Net.Http.Json;
using Adis.Bll.Dtos;
using Adis.Bll.Dtos.Auth;
using Adis.Bll.Dtos.Project;
using Adis.Dal.Data;
using Adis.Dm;
using Adis.Tests.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Adis.Tests
{
    /// <summary>
    /// Позволяет тестировать работу с проектами в API
    /// </summary>
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

        /// <summary>
        /// Создает пользователей для тестов
        /// </summary>
        /// <exception cref="Exception">Выбрасывается если пользователь не создался</exception>
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

        /// <summary>
        /// Возвращает токен доступа для авторизации
        /// </summary>
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

        /// <summary>
        /// Тестирует создания проекта с валидными данными
        /// </summary>
        [Fact]
        public async Task AddProject_ValidData_ReturnsCreatedProject()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

            var project = new PostProjectDto
            {
                Name = "Test Project",
                EndDate = new DateOnly(2024, 12, 31),
                IdUser = 1 // Должен соответствовать ID созданного пользователя
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/projects", project);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<PostProjectDto>();
            Assert.Equal(project.Name, result!.Name);
        }

        /// <summary>
        /// Тестирует авторизацию при создании проекта
        /// </summary>
        [Fact]
        public async Task AddProject_Unauthorized_ReturnsUnauthorized()
        {
            // Arrange
            var project = new PostProjectDto
            {
                Name = "Test Project",
                EndDate = new DateOnly(2024, 12, 31),
                IdUser = 1
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/projects", project);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        /// <summary>
        /// Тестирует фильтрацию при возвращения списка проектов
        /// </summary>
        [Fact]
        public async Task GetProjects_WithFilters_ReturnsFilteredResults()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            // Add test projects
            var projects = new List<PostProjectDto>
            {
                new()
                {
                    Name = "Filtered Project",
                    EndDate = new DateOnly(2024, 3, 31),
                    IdUser = 1
                },
                new()
                {
                    Name = "Other Project",
                    EndDate = new DateOnly(2024, 12, 31),
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
            Assert.Single(result!.Projects);
            Assert.Equal("Filtered Project", result.Projects.First().Name);
        }

        /// <summary>
        /// Тестирует создания проекта с невалидным бюджетом
        /// </summary>
        [Fact]
        public async Task AddProject_InvalidBudget_ReturnsValidationError()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

            var project = new PostProjectDto
            {
                Name = "Test Project",
                EndDate = new DateOnly(2024, 12, 31),
                IdUser = 1
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/projects", project);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            Assert.NotNull(problemDetails);
            Assert.Contains("Budget", problemDetails.Errors.Keys);
        }
    }
}