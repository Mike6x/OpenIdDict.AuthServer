using Identity.Application.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Users.Endpoints.ManagementExtensions;

public static class LockUserEndpoint
{
    internal static RouteHandlerBuilder MapLockUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/{userId}/lock", (string userId, IUserService service,CancellationToken cancellationToken) 
                => service.LockUserAsync(userId,cancellationToken))
                    .WithName(nameof(LockUserEndpoint))
                    .WithSummary("Lock user for 30 days")
                    // .RequirePermission("Permissions.Handlers.Remove")
                    .WithDescription("Lock user");
    }
}