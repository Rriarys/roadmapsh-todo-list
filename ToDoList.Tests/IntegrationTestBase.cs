using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;

namespace ToDoList.Api.Tests;

public abstract class IntegrationTestBase
{
    protected IntegrationTestBase(CustomWebApplicationFactory<Program> factory)
    {
        Client = factory.CreateClient();
    }

    protected HttpClient Client { get; }

    protected async Task<AuthCredentials> AuthenticateAsync()
    {
        var email = $"user-{Guid.NewGuid():N}@example.com";
        const string password = "Password123";

        var registerResponse = await Client.PostAsJsonAsync(
            "/api/auth/register",
            new
            {
                name = "Integration Test User",
                email,
                password
            });

        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

        var tokenResponse = await ReadRequiredAsync<AuthTokenPayload>(registerResponse);

        Assert.False(string.IsNullOrWhiteSpace(tokenResponse.Token));

        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenResponse.Token);

        return new AuthCredentials(email, password, tokenResponse.Token);
    }

    protected async Task<TodoPayload> CreateTodoAsync(
        string title,
        string? description = null)
    {
        var response = await Client.PostAsJsonAsync(
            "/api/todos",
            new
            {
                title,
                description
            });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var location = response.Headers.Location;

        Assert.NotNull(location);
        Assert.True(location.IsAbsoluteUri || location.OriginalString.StartsWith("/api/todos/"));

        var todo = await ReadRequiredAsync<TodoPayload>(response);

        Assert.Equal(title, todo.Title);
        Assert.Equal(description, todo.Description);

        return todo;
    }

    protected static async Task<TResponse> ReadRequiredAsync<TResponse>(
        HttpResponseMessage response)
    {
        var payload = await response.Content.ReadFromJsonAsync<TResponse>();

        Assert.NotNull(payload);

        return payload;
    }

    protected static async Task AssertProblemDetailsAsync(
        HttpResponseMessage response,
        HttpStatusCode expectedStatusCode)
    {
        Assert.Equal(expectedStatusCode, response.StatusCode);

        var responseBody = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            return;
        }

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.Equal((int)expectedStatusCode, problemDetails.Status);
        Assert.False(string.IsNullOrWhiteSpace(problemDetails.Title));
    }

    protected sealed record AuthCredentials(
        string Email,
        string Password,
        string Token);

    protected sealed record AuthTokenPayload(string Token);

    protected sealed record TodoPayload(
        Guid Id,
        string Title,
        string? Description);

    protected sealed record PagedTodoPayload(
        IReadOnlyCollection<TodoPayload> Items,
        int TotalCount,
        int Page,
        int PageSize);
}