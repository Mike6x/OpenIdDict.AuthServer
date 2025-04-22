using FluentValidation;
using Identity.Application.Claims.Features.Update;
using Identity.Application.Roles;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Roles.Endpoints.Claim;
public static class UpdateRoleClaimsEndpoint
{
    public static RouteHandlerBuilder MapUpdateRoleClaimsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPut("/{roleId}/claims", async (
            string roleId,
            AssignClaimsCommand request,
            IRoleService roleService,
            [FromServices] IValidator<AssignClaimsCommand> validator,
            CancellationToken cancellationToken) =>
        {
            if (roleId != request.Owner) return Results.BadRequest();
            var response = await roleService.UpdateClaimsToRoleAsync(roleId,request,cancellationToken);
            return Results.Ok(response);
        })
        .WithName(nameof(UpdateRoleClaimsEndpoint))
        .WithSummary("update role Claims")
        // .RequirePermission("Claims.Endpoints.Create")
        .WithDescription("update role Claims");
    }
}
