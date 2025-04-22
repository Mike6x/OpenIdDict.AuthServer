using Identity.Application.Roles;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Roles.Endpoints.Claim;

public static class GetRoleClaimsEndpoint
{
    public static RouteHandlerBuilder MapGetRoleWithClaimsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{roleId}/claims", async (
                string roleId, 
                IRoleService roleService,
                CancellationToken cancellationToken) 
                => await roleService.GetRoleWithClaimsAsync(roleId, cancellationToken))
                                        .WithName(nameof(GetRoleClaimsEndpoint))
                                        .WithSummary("Get role details with claim  by ID")
                                        // .RequirePermission("Permissions.Endpoints.View")
                                        .WithDescription("Retrieve the details of a role by its ID.");
    }
}
