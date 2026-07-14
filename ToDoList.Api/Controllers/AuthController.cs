using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ToDoList.Api.DTOs;
using ToDoList.Api.Services.Auth;

namespace ToDoList.Api.Controllers;

[ApiController]
[Route("api/auth")]
[EnableRateLimiting("auth-policy")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// POST /api/auth/register
    /// Registers a new user and returns an authentication token on success.
    /// </summary>
    /// <param name="request">The registration details payload.</param>
    /// <param name="ct">The cancellation token for the request.</param>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthTokenResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var response = await authService.RegisterAsync(request, ct);
        return Ok(response);
    }

    /// <summary>
    /// POST /api/auth/login
    /// Authenticates a user and returns an authentication token on success.
    /// </summary>
    /// <param name="request">The login credentials payload.</param>
    /// <param name="ct">The cancellation token for the request.</param>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthTokenResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var response = await authService.LoginAsync(request, ct);
        return Ok(response);
    }
}