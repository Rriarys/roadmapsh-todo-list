using System.Security.Claims;

namespace ToDoList.Api.Services.Auth;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private readonly HttpContext? _context = httpContextAccessor.HttpContext;

    public Guid UserId
    {
        get
        {
            var userIdString = _context?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
        }
    }

    public bool IsAuthenticated => _context?.User?.Identity?.IsAuthenticated ?? false;
}