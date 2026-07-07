namespace ToDoList.Api.DTOs;

public record TodoItemResponse(
    Guid Id, 
    string Title,
    string? Description
    );
