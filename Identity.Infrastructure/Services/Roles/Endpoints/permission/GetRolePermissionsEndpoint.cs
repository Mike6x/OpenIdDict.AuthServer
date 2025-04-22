using Identity.Application.Roles;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Roles.Endpoints.permission;
public static class GetRoleWithPermissionsEndpoint
{
    public static RouteHandlerBuilder MapGetRoleWithPermissionsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{roleId:guid}/permissions", async (
                string roleId, 
                IRoleService roleService, 
                CancellationToken cancellationToken) =>
                {
                    return await roleService.GetRoleWithPermissionsAsync(roleId, cancellationToken);
                })
                .WithName(nameof(GetRoleWithPermissionsEndpoint))
                .WithSummary("get role with permissions")
                // .RequirePermission("Permissions.Endpoints.View")
                .WithDescription("get role permissions");
    }
}
