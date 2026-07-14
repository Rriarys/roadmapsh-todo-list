using Microsoft.AspNetCore.Mvc;
using ToDoList.Api.DTOs;
using ToDoList.Api.Services.Auth;

namespace ToDoList.Api.Controllers;

[ApiController]
public class AuthController(
    IAuthService authService,
    CancellationToken ct) : ControllerBase
{
    [HttpPost("/register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        try
        {
            var response = await authService.RegisterAsync(request, ct);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message); // 409
        }
    }

    [HttpPost("/login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        try
        {
            var response = await authService.LoginAsync(request, ct);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message); // 401
        }
    }
}
