using ToDoList.Api.DTOs;

namespace ToDoList.Api.Services.Auth;

public interface IAuthService
{
    /// <summary>
    /// Registers a new user with the provided registration details and returns an authentication token upon successful registration.
    /// </summary>
    /// <param name="request">The registration details of the new user.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>An authentication token for the newly registered user.</returns>
    Task<AuthTokenResponse> RegisterAsync(RegisterRequest request, CancellationToken ct);

    /// <summary>
    /// Authenticates a user with the provided login details and returns an authentication token upon successful login.
    /// </summary>
    /// <param name="request">The login details of the user.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>An authentication token for the authenticated user.</returns>
    Task<AuthTokenResponse> LoginAsync(LoginRequest request, CancellationToken ct);
}