using Identity.Application.Claims.Features.Add;
using Identity.Application.Roles;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Roles.Endpoints.Claim;

public static class AddClaimToRoleEndpoint
{
    internal static RouteHandlerBuilder MapAddClaimToRoleEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/{roleId}/claim", async (
                string roleId,
                AddClaimCommand request,
                IRoleService service,
                CancellationToken cancellationToken) =>
            {
                if (roleId != request.Owner) return Results.BadRequest();
                
                var message = await service.AddClaimToRoleAsync(roleId, request, cancellationToken);
                return Results.Ok(message);
            })
            .WithName(nameof(AddClaimToRoleEndpoint))
            .WithSummary("Add a claim to Role")
            .WithDescription("Add a claim to Role");
    }

}
