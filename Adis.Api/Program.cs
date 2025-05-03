using Adis.Bll.Configurations;
using Adis.Bll.Initializers;
using Adis.Bll.Interfaces;
using Adis.Bll.Profiles;
using Adis.Bll.Services;
using Adis.Dal.Data;
using Adis.Dal.Interfaces;
using Adis.Dal.Repositories;
using Adis.Dm;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString),
        mysqlOptions =>
        {
            mysqlOptions.EnableRetryOnFailure(
                maxRetryCount: 10,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        })
    .LogTo(Console.WriteLine, LogLevel.Information)
    .EnableSensitiveDataLogging()
    .EnableDetailedErrors();
});

builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Настройка аутентификации через JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = new JwtSettings();
    builder.Configuration.GetSection("JWT").Bind(jwtSettings);

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,

        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,

        ValidateLifetime = true,

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.Key))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddCors();

builder.Services.AddAutoMapper(typeof(ProjectProfile), typeof(UserProfile));

builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectService, ProjectService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWT"));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

builder.Services.Configure<AdminSettings>(builder.Configuration.GetSection("AdminSettings"));
builder.Services.AddScoped<IAdminInitializer, AdminInitializer>();

builder.Services.AddSwaggerGen(options =>
{
    var basePath = AppContext.BaseDirectory;

    var xmlPath = Path.Combine(basePath, "Adis.Api.xml");
    options.IncludeXmlComments(xmlPath);

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"Введите JWT токен авторизации.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
          new OpenApiSecurityScheme
          {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            },
          },
          new List<string>()
        }
    });
});

var app = builder.Build();

// Инициализация базы данных с повторными попытками
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var dbContext = services.GetRequiredService<AppDbContext>();

    int retries = 0;
    const int maxRetries = 10;

    while (retries < maxRetries)
    {
        try
        {
            logger.LogInformation("Database connection attempt #{Retry}", retries + 1);

            if (!dbContext.Database.CanConnect())
                throw new Exception("Database connection failed");

            logger.LogInformation("Applying migrations...");
            dbContext.Database.Migrate();

            logger.LogInformation("Initializing admin user...");
            var adminInitializer = services.GetRequiredService<IAdminInitializer>();
            await adminInitializer.InitializeAsync();

            break;
        }
        catch (Exception ex)
        {
            retries++;
            logger.LogError(ex, "Database initialization failed");

            if (retries >= maxRetries)
            {
                logger.LogCritical("Database initialization failed after {MaxRetries} attempts", maxRetries);
                throw;
            }

            await Task.Delay(10000 * retries);
        }
    }
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseCors(builder =>
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader());

app.UseSwagger();
app.UseSwaggerUI();

app.Run();

// Требуется для создания тестов
#pragma warning disable CS1591 // Отсутствует комментарий XML для открытого видимого типа или члена
public partial class Program { }
#pragma warning restore CS1591 // Отсутствует комментарий XML для открытого видимого типа или члена