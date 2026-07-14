using ToDoList.Api.Models;

namespace ToDoList.Api.Services.Token;

public interface ITokenService
{
    string GenerateToken(User user);
}
