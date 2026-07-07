namespace ToDoList.Api.Models;

public class TodoItem
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;
    public string? Description { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
