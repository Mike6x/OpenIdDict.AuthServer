using Identity.Application.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Users.Endpoints.Roles;
public static class GetUserRolesEndpoint
{
    internal static RouteHandlerBuilder MapGetUserRolesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{userId}/roles", (string userId, IUserService service, CancellationToken cancellationToken) 
                => service.GetUserRolesAsync(userId, cancellationToken))
        .WithName(nameof(GetUserRolesEndpoint))
        .WithSummary("get user roles")
        // .RequirePermission("Permissions.Handlers.View")
        .WithDescription("get user roles");
    }
}
