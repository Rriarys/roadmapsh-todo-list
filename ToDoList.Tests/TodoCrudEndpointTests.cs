using System.Net;
using System.Net.Http.Json;

namespace ToDoList.Api.Tests;

public sealed class TodoCrudEndpointTests(
    CustomWebApplicationFactory<Program> factory)
    : IntegrationTestBase(factory),
        IClassFixture<CustomWebApplicationFactory<Program>>
{
    [Fact]
    public async Task CreateTodo_Authenticated_Returns201Created()
    {
        await AuthenticateAsync();

        var todo = await CreateTodoAsync(
            "Authenticated Todo",
            "Todo description");

        Assert.NotEqual(Guid.Empty, todo.Id);
    }

    [Fact]
    public async Task CreateTodo_Anonymous_Returns401Unauthorized()
    {
        Client.DefaultRequestHeaders.Authorization = null;

        var response = await Client.PostAsJsonAsync(
            "/api/todos",
            new
            {
                title = "Anonymous Todo",
                description = "Should not be created"
            });

        await AssertProblemDetailsAsync(
            response,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateTodo_EmptyTitle_Returns400BadRequest()
    {
        await AuthenticateAsync();

        var response = await Client.PostAsJsonAsync(
            "/api/todos",
            new
            {
                title = string.Empty,
                description = "Invalid todo"
            });

        await AssertProblemDetailsAsync(
            response,
            HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetTodoById_ExistingOwnedItem_Returns200OK()
    {
        await AuthenticateAsync();

        var createdTodo = await CreateTodoAsync("Owned Todo", "Description");

        var response = await Client.GetAsync($"/api/todos/{createdTodo.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var todo = await ReadRequiredAsync<TodoPayload>(response);

        Assert.Equal(createdTodo.Id, todo.Id);
        Assert.Equal(createdTodo.Title, todo.Title);
        Assert.Equal(createdTodo.Description, todo.Description);
    }

    [Fact]
    public async Task GetTodoById_NonExistentOrNotOwned_Returns404NotFound()
    {
        await AuthenticateAsync();

        var response = await Client.GetAsync($"/api/todos/{Guid.NewGuid()}");

        await AssertProblemDetailsAsync(
            response,
            HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateTodo_ExistingOwnedItem_Returns200OK()
    {
        await AuthenticateAsync();

        var createdTodo = await CreateTodoAsync("Original Title", "Original Description");

        var response = await Client.PutAsJsonAsync(
            $"/api/todos/{createdTodo.Id}",
            new
            {
                title = "Updated Title",
                description = "Updated Description"
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updatedTodo = await ReadRequiredAsync<TodoPayload>(response);

        Assert.Equal(createdTodo.Id, updatedTodo.Id);
        Assert.Equal("Updated Title", updatedTodo.Title);
        Assert.Equal("Updated Description", updatedTodo.Description);
    }

    [Fact]
    public async Task UpdateTodo_NonExistentOrNotOwned_Returns404NotFound()
    {
        await AuthenticateAsync();

        var response = await Client.PutAsJsonAsync(
            $"/api/todos/{Guid.NewGuid()}",
            new
            {
                title = "Ghost Todo",
                description = "Should not be updated"
            });

        await AssertProblemDetailsAsync(
            response,
            HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTodo_ExistingOwnedItem_Returns204NoContent()
    {
        await AuthenticateAsync();

        var createdTodo = await CreateTodoAsync("Todo To Delete");

        var deleteResponse =
            await Client.DeleteAsync($"/api/todos/{createdTodo.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse =
            await Client.GetAsync($"/api/todos/{createdTodo.Id}");

        await AssertProblemDetailsAsync(
            getResponse,
            HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTodo_NonExistentOrNotOwned_Returns404NotFound()
    {
        await AuthenticateAsync();

        var response =
            await Client.DeleteAsync($"/api/todos/{Guid.NewGuid()}");

        await AssertProblemDetailsAsync(
            response,
            HttpStatusCode.NotFound);
    }
}