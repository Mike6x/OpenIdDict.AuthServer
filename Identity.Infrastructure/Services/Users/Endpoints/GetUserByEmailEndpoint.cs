using Framework.Infrastructure.Auth.Policy;
using Identity.Application.Users.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Users.Endpoints;
public static class GetUserByEmailEndpoint
{
    internal static RouteHandlerBuilder MapGetUserByEmailEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/email/{email}", (string email, IUserService service) =>
        {
            return service.GetByEmailAsync(email, CancellationToken.None);
        })
        .WithName(nameof(GetUserByEmailEndpoint))
        .WithSummary("Get user profile by Name")
        .RequirePermission("Permissions.Endpoints.View")
        .WithDescription("Get another user's profile details by userName.");
    }
}
