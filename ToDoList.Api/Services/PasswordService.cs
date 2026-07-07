using Microsoft.AspNetCore.Identity;
using ToDoList.Api.Models;

namespace ToDoList.Api.Services;

public class PasswordService : IPasswordService
{
    private readonly PasswordHasher<User> _hasher = new();

    /// <inheritdoc />
    public string HashPassword(string plainPassword)
    {
        return _hasher.HashPassword(default!, plainPassword);
    }
    
    /// <inheritdoc />
    public bool VerifyPassword(string plainPassword, string hashedPassword)
    {
        return _hasher.VerifyHashedPassword(default!, hashedPassword, plainPassword) == PasswordVerificationResult.Success;
    }
}
