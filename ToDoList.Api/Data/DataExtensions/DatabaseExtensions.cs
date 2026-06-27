using Microsoft.EntityFrameworkCore;

namespace ToDoList.Api.Data.DataExtensions;

public static class DatabaseExtensions
{
    public static void ApplyMigrations(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToDoListDbContext>();
        dbContext.Database.Migrate();

        //SeedData(dbContext);
    }
}