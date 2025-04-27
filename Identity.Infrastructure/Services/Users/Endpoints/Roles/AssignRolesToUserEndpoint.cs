using Identity.Application.Users;
using Identity.Application.Users.Features.AssignUserRole;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Users.Endpoints.Roles;
public static class AssignRolesToUserEndpoint
{
    internal static RouteHandlerBuilder MapAssignRolesToUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/{userId}/roles", async (AssignUserRoleCommand command,
            string userId,
            IUserService userService,
            CancellationToken cancellationToken) =>
        {

            var message = await userService.AssignRolesToUserAsync(userId, command, cancellationToken);
            return Results.Ok(message);
        })
        .WithName(nameof(AssignRolesToUserEndpoint))
        .WithSummary("assign roles to a user")
        .WithDescription("assign roles to a user");
    }

}
