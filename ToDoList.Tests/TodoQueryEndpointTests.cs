using System.Net;

namespace ToDoList.Api.Tests;

public sealed class TodoQueryEndpointTests(
    CustomWebApplicationFactory<Program> factory)
    : IntegrationTestBase(factory),
        IClassFixture<CustomWebApplicationFactory<Program>>
{
    [Fact]
    public async Task ListTodos_DefaultPagination_ReturnsPagedResponse()
    {
        await AuthenticateAsync();

        await CreateTodoAsync("Task A");
        await CreateTodoAsync("Test B");
        await CreateTodoAsync("Task C");

        var response =
            await Client.GetAsync("/api/todos?page=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await ReadRequiredAsync<PagedTodoPayload>(response);

        Assert.NotNull(payload.Items);
        Assert.True(payload.TotalCount >= 3);
        Assert.Equal(1, payload.Page);
        Assert.Equal(10, payload.PageSize);
    }

    [Fact]
    public async Task ListTodos_FilterByTitle_ReturnsOnlyMatchingItems()
    {
        await AuthenticateAsync();

        await CreateTodoAsync("Test B");
        await CreateTodoAsync("Task A");
        await CreateTodoAsync("Another Task");

        var response =
            await Client.GetAsync("/api/todos?title=Test&page=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await ReadRequiredAsync<PagedTodoPayload>(response);

        Assert.NotEmpty(payload.Items);
        Assert.All(
            payload.Items,
            todo => Assert.Contains("Test", todo.Title));
    }

    [Fact]
    public async Task ListTodos_SortAscending_ReturnsSortedItems()
    {
        await AuthenticateAsync();

        await CreateTodoAsync("Task C");
        await CreateTodoAsync("Task A");
        await CreateTodoAsync("Task B");

        var response = await Client.GetAsync(
            "/api/todos?sortBy=title&sortDescending=false&page=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await ReadRequiredAsync<PagedTodoPayload>(response);

        var titles = payload.Items
            .Select(todo => todo.Title)
            .ToArray();

        Assert.Equal(
            titles.Order(StringComparer.Ordinal),
            titles);
    }

    [Fact]
    public async Task ListTodos_SortDescending_ReturnsSortedItems()
    {
        await AuthenticateAsync();

        await CreateTodoAsync("Task A");
        await CreateTodoAsync("Task C");
        await CreateTodoAsync("Task B");

        var response = await Client.GetAsync(
            "/api/todos?sortBy=title&sortDescending=true&page=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await ReadRequiredAsync<PagedTodoPayload>(response);

        var titles = payload.Items
            .Select(todo => todo.Title)
            .ToArray();

        Assert.Equal(
            titles.OrderDescending(StringComparer.Ordinal),
            titles);
    }

    [Fact]
    public async Task ListTodos_CombinedFilterAndSort_ReturnsCorrectlyScopedItems()
    {
        await AuthenticateAsync();

        await CreateTodoAsync("Task A");
        await CreateTodoAsync("Task C");
        await CreateTodoAsync("Test B");

        var response = await Client.GetAsync(
            "/api/todos?title=Task&sortBy=title&sortDescending=true&page=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await ReadRequiredAsync<PagedTodoPayload>(response);

        var titles = payload.Items
            .Select(todo => todo.Title)
            .ToArray();

        Assert.NotEmpty(titles);
        Assert.All(
            titles,
            title => Assert.Contains("Task", title));

        Assert.Equal(
            titles.OrderDescending(StringComparer.Ordinal),
            titles);
    }
}