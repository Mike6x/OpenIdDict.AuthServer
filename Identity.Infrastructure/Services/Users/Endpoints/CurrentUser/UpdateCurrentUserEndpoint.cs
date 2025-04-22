using System.Security.Claims;
using Framework.Core.Exceptions;
using Identity.Application.Users.Abstractions;
using Identity.Application.Users.Features.UpdateUser;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.Authorization;

namespace Identity.Infrastructure.Services.Users.Endpoints.CurrentUser;
public static class UpdateCurrentUserEndpoint
{
    internal static RouteHandlerBuilder MapUpdateCurrentUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPut("/profile/current", (
            UpdateUserCommand request, 
            ISender mediator, 
            ClaimsPrincipal user, 
            IUserService service) =>
        {
            if (user.GetUserId() is not { } userId || string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedException();
            }

            return service.UpdateAsync(request, userId);
        })
        .WithName(nameof(UpdateCurrentUserEndpoint))
        .WithSummary("update current user profile")
        // .RequirePermission("Permissions.Endpoints.Update")
        .WithDescription("Update profile of currently logged in user.");
    }
}
