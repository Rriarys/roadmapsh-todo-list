namespace ToDoList.Api.Services;

public interface IPasswordService
{
    /// <summary>
    /// Hashes the provided plain password using a secure hashing algorithm.
    /// </summary>
    /// <param name="plainPassword">The plain password to hash.</param>
    /// <returns>The hashed password.</returns>
    string HashPassword(string plainPassword);

    /// <summary>
    /// Verifies if the provided plain password matches the hashed password.
    /// </summary>
    /// <param name="plainPassword">The plain password to verify.</param>
    /// <param name="hashedPassword">The hashed password to compare against.</param>
    /// <returns>True if the passwords match; otherwise, false.</returns>
    bool VerifyPassword(string plainPassword, string hashedPassword);
}
