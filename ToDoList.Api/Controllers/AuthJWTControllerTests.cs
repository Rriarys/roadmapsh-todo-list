using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Api.Models;
using ToDoList.Api.Services;

namespace ToDoList.Api.Controllers;

// Only for JWT token generation and current user Id claim
[ApiController]
[Route("api/test")]
public class AuthJWTControllerTests(
    ITokenService tokenService,
    ICurrentUserService currentUserService) : ControllerBase
{
    [HttpPost("simulate-auth")]
    public IActionResult SimulateFlow()
    {
        // mock user
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            Name = "Tester"
        };

        var token = tokenService.GenerateToken(user);

        return Ok(new { Token = token, Message = "Token successfully generated" });
    }

    [Authorize]
    [HttpGet("protected")]
    public IActionResult GetProtectedData()
    {
        return Ok("Success: You are authorized!");
    }

    [HttpGet("unauthorized")]
    public IActionResult GetPublicData()
    {
        return Ok("This is a public method, accessible to everyone");
    }

    // ID
    [Authorize]
    [HttpGet("whoami")]
    public IActionResult GetCurrentUser()
    {
        if (!currentUserService.IsAuthenticated)
        {
            return Unauthorized("Not authenticated");
        }

        return Ok(new
        {
            UserId = currentUserService.UserId
        });
    }
}