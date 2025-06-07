using Adis.Bll.Configurations;
using Adis.Bll.Initializers;
using Adis.Bll.Interfaces;
using Adis.Bll.Profiles;
using Adis.Bll.Services;
using Adis.Dal.Data;
using Adis.Dal.Interfaces;
using Adis.Dal.Repositories;
using Adis.Dm;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options => {
    options.JsonSerializerOptions.Converters.Add(new NetTopologySuite.IO.Converters.GeoJsonConverterFactory());
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), options => options.UseNetTopologySuite());
});

builder.Services.AddIdentity<User, AppRole>()
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

builder.Services.AddAutoMapper(typeof(ProjectProfile), typeof(UserProfile), typeof(WorkObjectProfile), typeof(TaskProfile), typeof(DocumentProfile), typeof(CommentProfile), typeof(ExecutionTaskProfile));

builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectService, ProjectService>();

builder.Services.AddScoped<IRoleRepository, RoleRepository>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWT"));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

builder.Services.Configure<AdminSettings>(builder.Configuration.GetSection("AdminSettings"));
builder.Services.AddScoped<IAdminInitializer, AdminInitializer>();

builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();

builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();

builder.Services.AddScoped<IExecutionTaskService, ExecutionTaskService>();
builder.Services.AddScoped<IExecutionTaskRepository, ExecutionTaskRepository>();

builder.Services.AddScoped<IWorkObjectSectionService, WorkObjectSectionService>();
builder.Services.AddScoped<IWorkObjectSectionRepository, WorkObjectSectionRepository>();

builder.Services.AddScoped<INeuralGuideService, NeuralGuideService>();

builder.Services.AddMemoryCache();

builder.Services.Configure<OllamaSetting>(builder.Configuration.GetSection("Ollama"));

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

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10);
    options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(10);
});

var app = builder.Build();

// Инициализация базы данных с повторными попытками
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<AppDbContext>();

    if (!dbContext.Database.IsInMemory())
    {
        dbContext.Database.Migrate();

        var adminInitializer = services.GetRequiredService<IAdminInitializer>();
        await adminInitializer.InitializeAsync();

        var neuralGuideService = services.GetRequiredService<INeuralGuideService>();
        var documentService = services.GetRequiredService<IDocumentService>();

        await neuralGuideService.InitializeAsync(await documentService.GetGuideDocumentsAsync(), documentService.DirectoryPath);
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