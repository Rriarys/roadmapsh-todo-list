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
        Guid userId, int page, int pageSize, string? title, string? sortBy, bool sortDescending, CancellationToken ct)
    {
        // Start with a base query that filters by userId
        var queryableTodos = dbContext.TodoItems
            .AsNoTracking()
            .Where(t => t.UserId == userId);

        // Filter by title if provided
        if (!string.IsNullOrEmpty(title))
        {
            queryableTodos = queryableTodos.Where(t => t.Title.Contains(title));
        }

        // Dynamic sorting
        // Use a switch to allow sorting only by safe fields
        queryableTodos = sortBy?.ToLower() switch
        {
            "id" => sortDescending ? queryableTodos.OrderByDescending(t => t.Id) : queryableTodos.OrderBy(t => t.Id),
            "title" => sortDescending ? queryableTodos.OrderByDescending(t => t.Title) : queryableTodos.OrderBy(t => t.Title),
            // By default, sort by Title, then by Id for a deterministic result
            _ => queryableTodos.OrderBy(t => t.Title).ThenBy(t => t.Id)
        };

        // Get the total count after filtering
        var totalCount = await queryableTodos.CountAsync(ct);

        // Pagination (after sorting)
        var items = await queryableTodos
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }
}