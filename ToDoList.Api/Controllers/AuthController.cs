using Microsoft.AspNetCore.Mvc;
using ToDoList.Api.DTOs;
using ToDoList.Api.Services.Auth;

namespace ToDoList.Api.Controllers;

[ApiController]
[Route("api/auth")]
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
        try
        {
            var response = await authService.RegisterAsync(request, ct);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message }); // 409
        }
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
        try
        {
            var response = await authService.LoginAsync(request, ct);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message }); // 401
        }
    }
}