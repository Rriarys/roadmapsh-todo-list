using ToDoList.Api.DTOs;

namespace ToDoList.Api.Services.ToDo;

public interface ITodoService
{
    /// <summary>
    /// Creates a new todo item for the currently authenticated user.
    /// </summary>
    /// <param name="request">The details of the todo item to create.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>The details of the newly created todo item.</returns>
    Task<TodoItemResponse> CreateAsync(CreateTodoRequest request, CancellationToken ct);

    /// <summary>
    /// Retrieves a specific todo item by its ID, ensuring it belongs to the currently authenticated user.
    /// </summary>
    /// <param name="id">The ID of the todo item.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>The todo item details.</returns>
    Task<TodoItemResponse> GetByIdAsync(Guid id, CancellationToken ct);

    /// <summary>
    /// Retrieves a paginated list of todo items belonging to the currently authenticated user.
    /// </summary>
    /// <param name="query">The query parameters containing page index and page size.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A paginated list of todo items.</returns>
    Task<PagedTodoResponse> GetPagedListAsync(GetTodosQuery query, CancellationToken ct);

    /// <summary>
    /// Updates an existing todo item owned by the currently authenticated user.
    /// </summary>
    /// <param name="id">The ID of the todo item to update.</param>
    /// <param name="request">The updated details of the todo item.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>The updated todo item details.</returns>
    Task<TodoItemResponse> UpdateAsync(Guid id, UpdateTodoRequest request, CancellationToken ct);

    /// <summary>
    /// Deletes a todo item owned by the currently authenticated user.
    /// </summary>
    /// <param name="id">The ID of the todo item to delete.</param>
    /// <param name="ct">A cancellation token.</param>
    Task DeleteAsync(Guid id, CancellationToken ct);
}