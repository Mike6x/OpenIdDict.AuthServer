using Identity.Application.Users.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Users.Endpoints.ManagementExtensions;

public static class HasPasswordEndpoint
{
    internal static RouteHandlerBuilder MapHasPasswordEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/{userId}/haspassword", (string userId, IUserService service,CancellationToken cancellationToken) 
                => service.HasPasswordAsync(userId,cancellationToken))
                    .WithName(nameof(HasPasswordEndpoint))
                    .WithSummary("Check if user has password")
                    // .RequirePermission("Permissions.Endpoints.Delete")
                    .WithDescription("Check if user has password");
    }
}