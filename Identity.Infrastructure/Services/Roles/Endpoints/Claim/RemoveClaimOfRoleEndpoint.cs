using Identity.Application.Claims.DeleteClaim;
using Identity.Application.Roles;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Roles.Endpoints.Claim;

public static class RemoveClaimOfRoleEndpoint
{
    internal static RouteHandlerBuilder MapRemoveClaimOfRoleEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapDelete("/{roleId}/claim", async (
                string roleId,
                [FromBody]RemoveClaimCommand command,
                IRoleService service,
                CancellationToken cancellationToken) =>
            {
                if (roleId != command.Owner) return Results.BadRequest();
                var message = await service.RemoveClaimOfRoleAsync(roleId, command, cancellationToken);

                return Results.Ok(message);
            })
            .WithName(nameof(RemoveClaimOfRoleEndpoint))
            .WithSummary("Remove a claim from Role ")
            .WithDescription("Remove a claim from Role ");
    }
}