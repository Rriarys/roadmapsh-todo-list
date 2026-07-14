using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Api.DTOs;
using ToDoList.Api.Services.ToDo;

namespace ToDoList.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/todos")]
public class TodosController(ITodoService todoService) : ControllerBase
{
    /// <summary>
    /// POST /api/todos
    /// Creates a new to-do item for the current authenticated user.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TodoItemResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreateTodoRequest request, CancellationToken ct)
    {
        var createdTodo = await todoService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = createdTodo.Id }, createdTodo); // 201
    }

    /// <summary>
    /// GET /api/todos/{id}
    /// Retrieves a single to-do item by ID if it belongs to the authenticated user.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TodoItemResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var todo = await todoService.GetByIdAsync(id, ct);
        return Ok(todo); // 200
    }

    /// <summary>
    /// GET /api/todos
    /// Lists paginated to-do items belonging to the authenticated user.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedTodoResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPagedList([FromQuery] GetTodosQuery query, CancellationToken ct)
    {
        // GetTodosQuery is automatically bound from query parameters (e.g., ?page=1&pageSize=10)
        var pagedResult = await todoService.GetPagedListAsync(query, ct);
        return Ok(pagedResult); // 200
    }

    /// <summary>
    /// PUT /api/todos/{id}
    /// Updates an existing to-do item if the authenticated user is the owner.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TodoItemResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateTodoRequest request, CancellationToken ct)
    {
        var updatedTodo = await todoService.UpdateAsync(id, request, ct);
        return Ok(updatedTodo); // 200
    }

    /// <summary>
    /// DELETE /api/todos/{id}
    /// Immediately and physically deletes a to-do item if the authenticated user is the owner.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        await todoService.DeleteAsync(id, ct);
        return NoContent(); // 204
    }
}