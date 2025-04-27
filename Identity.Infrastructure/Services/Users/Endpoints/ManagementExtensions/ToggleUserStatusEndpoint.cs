using Identity.Application.Users;
using Identity.Application.Users.Features.ToggleUserStatus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Users.Endpoints.ManagementExtensions;

public static class ToggleUserStatusEndpoint
{
    internal static RouteHandlerBuilder MapToggleUserStatusEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/{userId}/toggle-active", async (
            string userId,
            ToggleUserStatusCommand command,
            [FromServices] IUserService userService,
            CancellationToken cancellationToken) =>
        {
            if (userId != command.UserId) return Results.BadRequest();
            
            await userService.SetActiveStatusAsync(command, cancellationToken);
            return Results.Ok();
        })
        .WithName(nameof(ToggleUserStatusEndpoint))
        .WithSummary("Toggle a user's active status")
        .WithDescription("Toggle a user's active status")
        .AllowAnonymous();
    }

}
