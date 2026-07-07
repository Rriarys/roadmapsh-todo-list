namespace ToDoList.Api.DTOs;

public record PagedTodoResponse(
    IReadOnlyCollection<TodoItemResponse> Items,
    int TotalCount,
    int Page,
    int PageSize
    );
