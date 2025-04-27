using Identity.Application.Roles;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Roles.Endpoints.Basic;
public static class GetRoleEndpoint
{
    public static RouteHandlerBuilder MapGetRoleEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{roleId:guid}", async (string roleId, IRoleService service) =>
            {
                return await service.GetAsync(roleId);
            })
            .WithName(nameof(GetRoleEndpoint))
            .WithSummary("Get role details without claims and permissions")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Retrieve the details of a role by its Id.");
    }
}
