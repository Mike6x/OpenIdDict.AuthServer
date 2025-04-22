using System.Security.Claims;
using Framework.Core.Exceptions;
using Identity.Application.Users.Abstractions;
using Shared.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Users.Endpoints;
public static class GetCurrentUserPermissionsEndpoint
{
    internal static RouteHandlerBuilder MapGetCurrentUserPermissionsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/permissions", async (ClaimsPrincipal user, IUserService service, CancellationToken cancellationToken) =>
        {
            if (user.GetUserId() is not { } userId || string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedException();
            }

            return await service.GetPermissionsAsync(userId, cancellationToken);
        })
        .WithName("GetUserPermissions")
        .WithSummary("Get current user permissions")
        .WithDescription("Get current user permissions");
    }
}
