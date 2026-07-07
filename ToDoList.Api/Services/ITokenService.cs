using ToDoList.Api.Models;

namespace ToDoList.Api.Services;

public interface ITokenService
{
    string GenerateToken(User user);
}
