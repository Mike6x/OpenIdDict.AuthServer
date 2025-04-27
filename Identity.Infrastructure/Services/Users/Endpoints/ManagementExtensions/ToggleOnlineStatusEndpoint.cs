using System.Security.Claims;
using Framework.Core.Exceptions;
using Identity.Application.Users;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.Authorization;

namespace Identity.Infrastructure.Services.Users.Endpoints.ManagementExtensions;
public static class ToggleOnlineStatusEndpoint
{
    internal static RouteHandlerBuilder MapToggleOnlineStatusEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/toggle-online", (
            ClaimsPrincipal user, 
            IUserService service, CancellationToken cancellationToken) =>
        {
            if (user.GetUserId() is not { } userId || string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedException();
            }
            return service.SetOnlineStatusAsync(userId, false, cancellationToken);
        })
        .WithName(nameof(ToggleOnlineStatusEndpoint))
        // .AllowAnonymous()
        .WithSummary("update online status")
        .WithDescription("Update profile of currently logged in user.");
    }
}
