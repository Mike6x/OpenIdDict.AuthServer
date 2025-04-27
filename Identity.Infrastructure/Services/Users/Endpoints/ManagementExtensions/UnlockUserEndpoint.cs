using Identity.Application.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Users.Endpoints.ManagementExtensions;

public static class UnLockUserEndpoint
{
    internal static RouteHandlerBuilder MapUnLockUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/{userId}/unlock", (string userId, IUserService service, CancellationToken cancellationToken) 
                =>  service.UnlockUserAsync(userId,cancellationToken))
                        .WithName(nameof(UnLockUserEndpoint))
                        .WithSummary("UnLock user")
                        // .RequirePermission("Permissions.Handlers.Remove")
                        .WithDescription("UnLock user");
    }
}