
using Identity.Application.Roles;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Roles.Endpoints;

public static class DeleteRoleEndpoint
{
    public static RouteHandlerBuilder MapDeleteRoleEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapDelete("/{roleId:guid}", async (string roleId, IRoleService roleService) =>
        {
            await roleService.DeleteAsync(roleId);
        })
        .WithName(nameof(DeleteRoleEndpoint))
        .WithSummary("Delete a role by ID")
        // .RequirePermission("Permissions.Endpoints.Delete")
        .WithDescription("Remove a role from the system by its ID.");
    }
}

