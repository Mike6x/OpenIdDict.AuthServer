using FluentValidation;
using Identity.Application.Claims.Features.Update;
using Identity.Application.Roles;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Roles.Endpoints.Claim;

public static class AssignClaimsToRoleEndpoint
{
    public static RouteHandlerBuilder MapAssignClaimsToRoleEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/{roleId}/claims", async (
                string roleId,
                AssignClaimsCommand request,
                IRoleService roleService,
                [FromServices] IValidator<AssignClaimsCommand> validator,
                CancellationToken cancellationToken) =>
            {
                if (roleId != request.Owner) return Results.BadRequest();
                var response = await roleService.AssignClaimsToRoleAsync(roleId,request,cancellationToken); //
                return Results.Ok(response);
            })
            .WithName(nameof(AssignClaimsToRoleEndpoint))
            .WithSummary("Assign role Claims")
            // .RequirePermission("Claims.Handlers.Create")
            .WithDescription("Assign/remove a list of Claims");
    }
}
