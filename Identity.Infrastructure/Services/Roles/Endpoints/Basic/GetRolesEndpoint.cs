using Identity.Application.Roles;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Roles.Endpoints;

public static class GetRolesEndpoint
{
    public static RouteHandlerBuilder MapGetRolesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/", async (IRoleService roleService, CancellationToken cancellationToken) =>
                await roleService.GetAllAsync(cancellationToken))
        .WithName(nameof(GetRolesEndpoint))
        .WithSummary("Get a list of all roles")
        // .RequirePermission("Permissions.Endpoints.View")
        .WithDescription("Retrieve a list of all roles available in the system.");
    }
}
