using System.ComponentModel.DataAnnotations;

namespace ToDoList.Api.DTOs;

public record UpdateTodoRequest(
    [Required(AllowEmptyStrings = false, ErrorMessage = "Title is required.")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
    string Title,

    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
    string? Description
    );
