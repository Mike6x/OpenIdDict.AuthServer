using System.Security.Claims;
using Framework.Core.Exceptions;
using Identity.Application.Users;
using Shared.Authorization;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Users.Endpoints.CurrentUser;
public static class ToggleOnlineStatusEndpoint
{
    internal static RouteHandlerBuilder MapToggleOnlineStatusEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/toggle-online", (
            ISender mediator, 
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
