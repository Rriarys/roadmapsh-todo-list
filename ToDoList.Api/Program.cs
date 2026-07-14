using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;
using ToDoList.Api.Data;
using ToDoList.Api.Data.DataExtensions;
using ToDoList.Api.Middleware;
using ToDoList.Api.Options;
using ToDoList.Api.Repositories;
using ToDoList.Api.Services.Auth;
using ToDoList.Api.Services.PassHash;
using ToDoList.Api.Services.ToDo;
using ToDoList.Api.Services.Token;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddRateLimiter(options =>
{
    // Global rate limit: 100 requests per minute for all endpoints
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter("global", _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 100, // 100 requests
            Window = TimeSpan.FromMinutes(1) // per minute
        }));

    // Strict policy for Auth
    options.AddFixedWindowLimiter("auth-policy", opt =>
    {
        opt.PermitLimit = 5; // Only 5 attempts
        opt.Window = TimeSpan.FromMinutes(1); // per minute
        opt.QueueLimit = 0; // Reject immediately
    });

    // Handling case when limit is exceeded (429 Too Many Requests)
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

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

builder.Services.AddScoped<IPasswordService, PasswordService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddAuthorization();

builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddScoped<ITodoService, TodoService>();

var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
{
    app.Services.ApplyMigrations();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
