using Identity.Application.Roles;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Roles.Endpoints.Claim;

public static class GetRoleClaimsEndpoint
{
    public static RouteHandlerBuilder MapGetRoleClaimsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{roleId}/claims", async (
                string roleId, 
                IRoleService roleService,
                CancellationToken cancellationToken) 
                => await roleService.GetRoleClaimsAsync(roleId, cancellationToken))
                                        .WithName(nameof(GetRoleClaimsEndpoint))
                                        .WithSummary("Get Role claims")
                                        // .RequirePermission("Permissions.Handlers.View")
                                        .WithDescription("Retrieve all claim of a role.");
    }
}
