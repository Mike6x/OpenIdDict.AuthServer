using Identity.Application.Claims.DeleteClaim;
using Identity.Application.Users.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Users.Endpoints.Claim;

public static class RemoveUserClaimEndpoint
{
    internal static RouteHandlerBuilder MapRemoveUserClaimEndpoint(this IEndpointRouteBuilder endpoints)
    {
            return endpoints.MapDelete("/{userId}/claim", async (
                string userId,
                [FromBody]RemoveClaimCommand command,
                IUserService service,
                CancellationToken cancellationToken) =>
            {
                if (userId != command.Owner) return Results.BadRequest();
                var message = await service.RemoveClaimOfUserAsync(userId, command, cancellationToken);

                return Results.Ok(message);
            })
            .WithName(nameof(RemoveUserClaimEndpoint))
            .WithSummary("Remove a claim from User ")
            .WithDescription("Remove a claim from User ");
    }
}
