using Identity.Application.Claims.Features.Change;
using Identity.Application.Roles;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Roles.Endpoints.Claim;

public static class ChangeClaimOfRoleEndpoint
{
    internal static RouteHandlerBuilder MapChangeClaimOfRoleEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPut("/{roleId}/claim", async (
                string roleId,
                ChangeClaimCommand command,
                IRoleService service,
                CancellationToken cancellationToken) =>
            {
                if (roleId != command.Owner) return Results.BadRequest();
                
                var message = await service.ChangeClaimOfRoleAsync(roleId, command, cancellationToken);
                return Results.Ok(message);
            })
            .WithName(nameof(ChangeClaimOfRoleEndpoint))
            .WithSummary("Change a Role claim to new")
            .WithDescription("Change a Role claim to new");
    }

}
