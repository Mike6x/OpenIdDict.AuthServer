using Identity.Application.Claims.Features.Add;
using Identity.Application.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Users.Endpoints.Claim;

public static class AddClaimToUserEndpoint
{
    internal static RouteHandlerBuilder MapAddClaimToUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/{userId}/claim", async (
                HttpContext context,
                string userId,
                AddClaimCommand command,
                IUserService service,
                CancellationToken cancellationToken) =>
            {
                if (userId != command.Owner) return Results.BadRequest();
                
                var message = await service.AddClaimToUserAsync(userId, command, cancellationToken);
                return Results.Ok(message);
            })
            .WithName(nameof(AddClaimToUserEndpoint))
            .WithSummary("Add a claim to User")
            .WithDescription("Add a claim to User");
    }

}
