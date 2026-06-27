using BloggingPlatform.API.Middlewares;
using Microsoft.EntityFrameworkCore;
using ToDoList.Api.Data;
using ToDoList.Api.Data.DataExtensions;

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

var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
{
    app.Services.ApplyMigrations();
}

app.MapControllers();

app.Run();
