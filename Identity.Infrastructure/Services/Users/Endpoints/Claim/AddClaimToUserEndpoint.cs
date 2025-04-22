using Identity.Application.Claims.Features.Add;
using Identity.Application.Users.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Users.Endpoints.Claim;

public static class AddUserClaimEndpoint
{
    internal static RouteHandlerBuilder MapAddUserClaimEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/{userId}/claim", async (
                HttpContext context,
                string userId,
                AddClaimCommand command,
                IUserService userService,
                CancellationToken cancellationToken) =>
            {
                if (userId != command.Owner) return Results.BadRequest();
                
                var message = await userService.AddClaimToUserAsync(userId, command, cancellationToken);
                return Results.Ok(message);
            })
            .WithName(nameof(AddUserClaimEndpoint))
            .WithSummary("Add a claim to User")
            .WithDescription("Add a claim to User");
    }

}
