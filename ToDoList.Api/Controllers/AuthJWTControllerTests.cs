using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Api.Models;
using ToDoList.Api.Services;

namespace ToDoList.Api.Controllers;

// Only for JWT token generation simulation and testing purposes
[ApiController]
[Route("api/test")]
public class AuthJWTControllerTests(ITokenService tokenService) : ControllerBase
{
    [HttpPost("simulate-auth")]
    public IActionResult SimulateFlow()
    {
        // 1. Create a mock user
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            Name = "Tester"
        };

        // 2. Generate a token for the mock user
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
}