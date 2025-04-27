using System.Security.Claims;

namespace Framework.Core.Identity.Users.Abstractions;
public interface ICurrentUserInitializer
{
    void SetCurrentUser(ClaimsPrincipal user);

    void SetCurrentUserId(string userId);
}
