using System.Security.Claims;
using Framework.Core.Exceptions;
using Identity.Application.Users.Abstractions;
using Shared.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;


namespace Identity.Infrastructure.Services.Users.Endpoints;
public static class GetCurrentUserEndpoint
{
    internal static RouteHandlerBuilder MapGetCurrentUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/profile", async (ClaimsPrincipal user, IUserService service, CancellationToken cancellationToken) =>
        {
            if (user.GetUserId() is not { } userId || string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedException();
            }

            return await service.GetAsync(userId, cancellationToken);
        })
        .WithName(nameof(GetCurrentUserEndpoint))
        .WithSummary("Get current user information based on token")
        .WithDescription("Get current user information based on token");
    }
}
