using Identity.Application.Claims.Features.Update;
using Identity.Application.Users.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Users.Endpoints.Claim;

public static class AssignUserClaimsEndpoint
{
    internal static RouteHandlerBuilder MapAssignUserClaimsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/{userId}/claims-update", (
                string userId, 
                UpdateClaimsCommand request,
                IUserService service,
                CancellationToken cancellationToken) =>
            {
                return service.AssignClaimsToUserAsync(userId, request, cancellationToken);
            })
            .WithName(nameof(AssignUserClaimsEndpoint))
            .WithSummary("Update claims")
            // .RequirePermission("Permissions.Endpoints.View")
            .WithDescription("Update list of claims to a user");
    }
}