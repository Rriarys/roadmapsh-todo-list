using Microsoft.EntityFrameworkCore;
using ToDoList.Api.Models;

namespace ToDoList.Api.Data.DataExtensions;

public static class DatabaseExtensions
{
    public static void ApplyMigrations(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToDoListDbContext>();
        dbContext.Database.Migrate();

        SeedData(dbContext);
    }

    private static void SeedData(ToDoListDbContext dbContext)
    {
        if (dbContext.TodoItems.Any())
        {
            return; // Database has already been seeded
        }

        var users = new List<User>
        {
            new()
            {
                Name = "John Doe",
                Email = "john@example.com",
                PasswordHash = "dummy-hash"
            },
            new()
            {
                Name = "Jane Smith",
                Email = "jane@example.com",
                PasswordHash = "dummy-hash"
            },
            new()
            {
                Name = "Michael Johnson",
                Email = "michael@example.com",
                PasswordHash = "dummy-hash"
            },
            new()
            {
                Name = "Emily Davis",
                Email = "emily@example.com",
                PasswordHash = "dummy-hash"
            },
            new()
            {
                Name = "David Wilson",
                Email = "david@example.com",
                PasswordHash = "dummy-hash"
            }
        };

        dbContext.Users.AddRange(users);

        var todoItems = new List<TodoItem>();
        var itemsAmount = 23; // Total number of items we want to create
        var random = new Random();
        var itemCounts = new List<int>(Enumerable.Repeat(1, users.Count));
        var remainingItems = itemsAmount - itemCounts.Count;

        for (var i = 0; i < remainingItems; i++)
        {
            itemCounts[random.Next(itemCounts.Count)]++;
        }

        for (var userIndex = 0; userIndex < users.Count; userIndex++)
        {
            var user = users[userIndex];

            for (var itemIndex = 0; itemIndex < itemCounts[userIndex]; itemIndex++)
            {
                todoItems.Add(new TodoItem
                {
                    Title = $"Task {userIndex + 1}-{itemIndex + 1}",
                    Description = $"Seeded item for {user.Name}",
                    User = user
                });
            }
        }

        dbContext.TodoItems.AddRange(todoItems);
        dbContext.SaveChanges();
    }
}
