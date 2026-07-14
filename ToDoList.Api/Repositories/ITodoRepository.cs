using ToDoList.Api.Models;

namespace ToDoList.Api.Repositories;

public interface ITodoRepository
{
    /// <summary>
    /// Retrieves a specific todo item for a specific user.
    /// </summary>
    /// <param name="id">The ID of the todo item.</param>
    /// <param name="userId">The ID of the user who owns the todo item.</param>
    /// <param name="trackChanges">Indicates whether to track changes for the retrieved entity.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>The todo item if found; otherwise, null.</returns>
    Task<TodoItem?> GetByIdAsync(Guid id, Guid userId, bool trackChanges, CancellationToken ct);

    /// <summary>
    /// Retrieves a paginated list of todo items for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user whose todo items are being retrieved.</param>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A tuple containing the list of todo items and the total count of items.</returns>
    Task<(IReadOnlyCollection<TodoItem> Items, int TotalCount)> GetPagedListAsync(
        Guid userId, int page, int pageSize, CancellationToken ct);

    /// <summary>
    /// Persists a new todo item to the database.
    /// </summary>
    /// <param name="todo">The todo item to create.</param>
    /// <param name="ct">A cancellation token.</param>
    Task CreateAsync(TodoItem todo, CancellationToken ct);

    /// <summary>
    /// Updates an existing todo item in the database.
    /// </summary>
    /// <param name="todo">The todo item to update.</param>
    /// <param name="ct">A cancellation token.</param>
    Task UpdateAsync(TodoItem todo, CancellationToken ct);

    /// <summary>
    /// Deletes a todo item from the database.
    /// </summary>
    /// <param name="todo">The todo item to delete.</param>
    /// <param name="ct">A cancellation token.</param>
    Task DeleteAsync(TodoItem todo, CancellationToken ct);
}