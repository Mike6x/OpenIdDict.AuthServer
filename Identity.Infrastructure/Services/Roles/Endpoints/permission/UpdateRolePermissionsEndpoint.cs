using FluentValidation;
using Identity.Application.Roles;
using Identity.Application.Roles.Features.UpdatePermissions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Roles.Endpoints.permission;
public static class UpdateRolePermissionsEndpoint
{
    public static RouteHandlerBuilder MapUpdateRolePermissionsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPut("/{roleId}/permissions", async (
            UpdatePermissionsCommand request,
            IRoleService roleService,
            string roleId,
            [FromServices] IValidator<UpdatePermissionsCommand> validator) =>
        {
            if (roleId != request.RoleId) return Results.BadRequest();
            var response = await roleService.UpdatePermissionsToRoleAsync(request);
            return Results.Ok(response);
        })
        .WithName(nameof(UpdateRolePermissionsEndpoint))
        .WithSummary("update role permissions")
        // .RequirePermission("Permissions.Handlers.Create")
        .WithDescription("update role permissions");
    }
}
