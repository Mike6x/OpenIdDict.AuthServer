using Identity.Application.Roles;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Roles.Endpoints.Basic;

public static class GetRolesEndpoint
{
    public static RouteHandlerBuilder MapGetRolesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/", async (IRoleService service, CancellationToken cancellationToken) =>
                await service.GetAllAsync(cancellationToken))
        .WithName(nameof(GetRolesEndpoint))
        .WithSummary("Get a list of all roles")
        // .RequirePermission("Permissions.Handlers.View")
        .WithDescription("Retrieve a list of all roles available in the system.");
    }
}
