using ToDoList.Api.Data;
using ToDoList.Api.DTOs;
using ToDoList.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ToDoList.Api.Services;

public class AuthService(
    ToDoListDbContext toDoListDbContext,
    IPasswordService passwordService,
    ITokenService tokenService) : IAuthService
{
    /// <inheritdoc />
    public async Task<AuthTokenResponse> RegisterAsync(RegisterRequest request)
    {
        if (await toDoListDbContext.Users.AnyAsync(user => user.Email == request.Email))
        {
            throw new InvalidOperationException("Email already in use.");
        }

        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            PasswordHash = passwordService.HashPassword(request.Password)
        };

        toDoListDbContext.Users.Add(newUser);
        await toDoListDbContext.SaveChangesAsync();

        return new AuthTokenResponse(tokenService.GenerateToken(newUser));
    }

    /// <inheritdoc />
    public async Task<AuthTokenResponse> LoginAsync(LoginRequest request)
    {
        var existingUser = await toDoListDbContext.Users
                    .FirstOrDefaultAsync(user => user.Email == request.Email);

        if (existingUser == null || !passwordService.VerifyPassword(request.Password, existingUser.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        return new AuthTokenResponse(tokenService.GenerateToken(existingUser));
    }
}
