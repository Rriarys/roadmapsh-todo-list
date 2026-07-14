using Microsoft.EntityFrameworkCore;
using ToDoList.Api.Data;
using ToDoList.Api.Models;

namespace ToDoList.Api.Repositories;

public class TodoRepository(ToDoListDbContext dbContext) : ITodoRepository
{
    /// <inheritdoc />
    public async Task<TodoItem?> GetByIdAsync(Guid id, Guid userId, bool trackChanges, CancellationToken ct)
    {
        var query = dbContext.TodoItems.AsQueryable();

        // Apply NoTracking optimization if read-only operation is requested
        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId, ct);
    }

    /// <inheritdoc />
    public async Task CreateAsync(TodoItem todo, CancellationToken ct)
    {
        await dbContext.TodoItems.AddAsync(todo, ct);
        await dbContext.SaveChangesAsync(ct);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(TodoItem todo, CancellationToken ct)
    {
        await dbContext.SaveChangesAsync(ct);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(TodoItem todo, CancellationToken ct)
    {
        dbContext.TodoItems.Remove(todo);
        await dbContext.SaveChangesAsync(ct);
    }

    /// <inheritdoc />
    public async Task<(IReadOnlyCollection<TodoItem> Items, int TotalCount)> GetPagedListAsync(
        Guid userId, int page, int pageSize, CancellationToken ct)
    {
        // Filter elements by the authenticated user's ID with tracking disabled for maximum performance
        var queryableTodos = dbContext.TodoItems
            .AsNoTracking()
            .Where(t => t.UserId == userId);

        // Apply deterministic ordering for reliable pagination
        queryableTodos = queryableTodos.OrderBy(t => t.Title).ThenBy(t => t.Id);

        // Fetch the total count of user's todo items matching the criteria
        var totalCount = await queryableTodos.CountAsync(ct);

        // Retrieve only the slice of items required for the requested page
        var items = await queryableTodos
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }
}