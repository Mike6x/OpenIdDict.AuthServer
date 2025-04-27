using Identity.Application.Claims.Features.Change;
using Identity.Application.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Users.Endpoints.Claim;

public static class ChangeClaimOfUserEndpoint
{
    internal static RouteHandlerBuilder MapChangeClaimOfUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPut("/{userId}/claim", async (
                HttpContext context,
                string userId,
                ChangeClaimCommand command,
                IUserService service,
                CancellationToken cancellationToken) =>
            {
                if (userId != command.Owner) return Results.BadRequest();
                
                var message = await service.ChangeClaimOfUserAsync(userId, command, cancellationToken);
                return Results.Ok(message);
            })
            .WithName(nameof(ChangeClaimOfUserEndpoint))
            .WithSummary("Change a claim to new")
            .WithDescription("Change a claim to new");
    }

}
