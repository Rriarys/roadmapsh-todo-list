using BloggingPlatform.API.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ToDoList.Api.Data;
using ToDoList.Api.Data.DataExtensions;
using ToDoList.Api.Options;
using ToDoList.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers();

string? SQLiteConnectionString = builder.Configuration.GetConnectionString("SQLiteConnection");
if (string.IsNullOrWhiteSpace(SQLiteConnectionString))
{
    throw new InvalidOperationException("Connection string 'SQLiteConnection' not found.");
}
builder.Services.AddDbContext<ToDoListDbContext>(options =>
    options.UseSqlite(SQLiteConnectionString));

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!))
        };
    });

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddAuthorization();

builder.Services.AddScoped<IPasswordService, PasswordService>();

var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
{
    app.Services.ApplyMigrations();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
