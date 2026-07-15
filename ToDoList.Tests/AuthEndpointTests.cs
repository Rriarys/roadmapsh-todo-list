using System.Net;
using System.Net.Http.Json;

namespace ToDoList.Api.Tests;

public sealed class AuthEndpointTests(
    CustomWebApplicationFactory<Program> factory)
    : IntegrationTestBase(factory),
        IClassFixture<CustomWebApplicationFactory<Program>>
{
    [Fact]
    public async Task Register_ValidData_Returns200OK()
    {
        var response = await Client.PostAsJsonAsync(
            "/api/auth/register",
            new
            {
                name = "Registered User",
                email = $"register-{Guid.NewGuid():N}@example.com",
                password = "Password123"
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await ReadRequiredAsync<AuthTokenPayload>(response);

        Assert.False(string.IsNullOrWhiteSpace(payload.Token));
    }

    [Fact]
    public async Task Register_DuplicateEmail_Returns409Conflict()
    {
        var email = $"duplicate-{Guid.NewGuid():N}@example.com";
        const string password = "Password123";

        var firstResponse = await Client.PostAsJsonAsync(
            "/api/auth/register",
            new
            {
                name = "Duplicate User",
                email,
                password
            });

        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);

        var duplicateResponse = await Client.PostAsJsonAsync(
            "/api/auth/register",
            new
            {
                name = "Duplicate User",
                email,
                password
            });

        await AssertProblemDetailsAsync(
            duplicateResponse,
            HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Login_ValidCredentials_Returns200OK()
    {
        var email = $"login-{Guid.NewGuid():N}@example.com";
        const string password = "Password123";

        var registerResponse = await Client.PostAsJsonAsync(
            "/api/auth/register",
            new
            {
                name = "Login User",
                email,
                password
            });

        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

        var loginResponse = await Client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                email,
                password
            });

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var payload = await ReadRequiredAsync<AuthTokenPayload>(loginResponse);

        Assert.False(string.IsNullOrWhiteSpace(payload.Token));
    }

    [Fact]
    public async Task Login_InvalidPassword_Returns401Unauthorized()
    {
        var email = $"invalid-password-{Guid.NewGuid():N}@example.com";

        var registerResponse = await Client.PostAsJsonAsync(
            "/api/auth/register",
            new
            {
                name = "Invalid Password User",
                email,
                password = "Password123"
            });

        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

        var loginResponse = await Client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                email,
                password = "WrongPassword"
            });

        await AssertProblemDetailsAsync(
            loginResponse,
            HttpStatusCode.Unauthorized);
    }
}