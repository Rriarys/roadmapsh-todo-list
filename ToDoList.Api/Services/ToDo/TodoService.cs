using ToDoList.Api.DTOs;
using ToDoList.Api.Models;
using ToDoList.Api.Repositories;
using ToDoList.Api.Services.Auth;

namespace ToDoList.Api.Services.ToDo;

public class TodoService(
    ITodoRepository todoRepository,
    ICurrentUserService currentUserService) : ITodoService
{
    /// <inheritdoc />
    public async Task<TodoItemResponse> CreateAsync(CreateTodoRequest request, CancellationToken ct)
    {
        var userId = currentUserService.UserId;

        var todo = new TodoItem
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = request.Title,
            Description = request.Description
        };

        await todoRepository.CreateAsync(todo, ct);

        return new TodoItemResponse(todo.Id, todo.Title, todo.Description);
    }

    /// <inheritdoc />
    public async Task<TodoItemResponse> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var userId = currentUserService.UserId;
        var todo = await todoRepository.GetByIdAsync(id, userId, trackChanges: false, ct);

        if (todo == null)
        {
            throw new KeyNotFoundException($"Todo item with ID '{id}' was not found.");
        }

        return new TodoItemResponse(todo.Id, todo.Title, todo.Description);
    }

    /// <inheritdoc />
    public async Task<PagedTodoResponse> GetPagedListAsync(GetTodosQuery query, CancellationToken ct)
    {
        var userId = currentUserService.UserId;

        var (items, totalCount) = await todoRepository.GetPagedListAsync(
            userId,
            query.Page,
            query.PageSize,
            ct);

        var projectedItems = items
            .Select(todo => new TodoItemResponse(todo.Id, todo.Title, todo.Description))
            .ToList();

        return new PagedTodoResponse(
            projectedItems,
            totalCount,
            query.Page,
            query.PageSize);
    }

    /// <inheritdoc />
    public async Task<TodoItemResponse> UpdateAsync(Guid id, UpdateTodoRequest request, CancellationToken ct)
    {
        var userId = currentUserService.UserId;
        var todo = await todoRepository.GetByIdAsync(id, userId, trackChanges: true, ct);

        if (todo == null)
        {
            throw new KeyNotFoundException($"Todo item with ID '{id}' was not found.");
        }

        todo.Title = request.Title;
        todo.Description = request.Description;

        await todoRepository.UpdateAsync(todo, ct);

        return new TodoItemResponse(todo.Id, todo.Title, todo.Description);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var userId = currentUserService.UserId;
        var todo = await todoRepository.GetByIdAsync(id, userId, trackChanges: true, ct);

        if (todo == null)
        {
            throw new KeyNotFoundException($"Todo item with ID '{id}' was not found.");
        }

        await todoRepository.DeleteAsync(todo, ct);
    }
}