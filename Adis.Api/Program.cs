using Adis.Bll.Identity;
using Adis.Bll.Interfaces;
using Adis.Bll.Profiles;
using Adis.Bll.Services;
using Adis.Dal.Data;
using Adis.Dal.Interfaces;
using Adis.Dal.Repositories;
using Adis.Dm;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddIdentityServer()
           .AddDeveloperSigningCredential() // Только для разработки!
           .AddInMemoryApiResources(Config.GetApiResources())
           .AddInMemoryClients(Config.GetClients())
           .AddInMemoryIdentityResources(Config.GetIdentityResources())
           .AddAspNetIdentity<User>()
           .AddProfileService<CustomProfileService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddAutoMapper(typeof(ProjectProfile), typeof(UserProfile));

builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectService, ProjectService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddSwaggerGen(options =>
{
    var basePath = AppContext.BaseDirectory;

    var xmlPath = Path.Combine(basePath, "Adis.Api.xml");
    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseIdentityServer();
app.UseAuthorization();
app.UseAuthorization();

app.MapControllers();

app.UseCors("AllowFrontend");

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
