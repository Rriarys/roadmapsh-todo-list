using ToDoList.Api.Data;
using ToDoList.Api.DTOs;
using ToDoList.Api.Models;
using Microsoft.EntityFrameworkCore;
using ToDoList.Api.Services.PassHash;
using ToDoList.Api.Services.Token;

namespace ToDoList.Api.Services.Auth;

public class AuthService(
    ToDoListDbContext toDoListDbContext,
    IPasswordService passwordService,
    ITokenService tokenService) : IAuthService
{
    /// <inheritdoc />
    public async Task<AuthTokenResponse> RegisterAsync(RegisterRequest request, CancellationToken ct)
    {
        if (await toDoListDbContext.Users.AnyAsync(user => user.Email == request.Email, ct))
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

        // rollback if the user drop the connection before the save changes
        await toDoListDbContext.SaveChangesAsync(ct);

        return new AuthTokenResponse(tokenService.GenerateToken(newUser));
    }

    /// <inheritdoc />
    public async Task<AuthTokenResponse> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var existingUser = await toDoListDbContext.Users
                    .FirstOrDefaultAsync(user => user.Email == request.Email, ct);

        if (existingUser == null || !passwordService.VerifyPassword(request.Password, existingUser.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        return new AuthTokenResponse(tokenService.GenerateToken(existingUser));
    }
}