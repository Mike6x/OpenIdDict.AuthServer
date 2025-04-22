
using Identity.Application.Roles;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Roles.Endpoints;
public static class GetRoleEndpoint
{
    public static RouteHandlerBuilder MapGetRoleEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{roleId:guid}", async (string roleId, IRoleService roleService) =>
            {
                return await roleService.GetAsync(roleId);
            })
            .WithName(nameof(GetRoleEndpoint))
            .WithSummary("Get role details without claims and permissions")
            // .RequirePermission("Permissions.Endpoints.View")
            .WithDescription("Retrieve the details of a role by its Id.");
    }
}
