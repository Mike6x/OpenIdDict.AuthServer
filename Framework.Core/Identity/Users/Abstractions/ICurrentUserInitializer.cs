using System.Security.Claims;

namespace Identity.Application.Users.Abstractions;
public interface ICurrentUserInitializer
{
    void SetCurrentUser(ClaimsPrincipal user);

    void SetCurrentUserId(string userId);
}
