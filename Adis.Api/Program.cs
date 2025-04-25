using Adis.Bll.Interfaces;
using Adis.Bll.Profiles;
using Adis.Bll.Services;
using Adis.Dal.Data;
using Adis.Dal.Interfaces;
using Adis.Dal.Repositories;
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

app.UseAuthorization();

app.MapControllers();

app.UseCors("AllowFrontend");

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
