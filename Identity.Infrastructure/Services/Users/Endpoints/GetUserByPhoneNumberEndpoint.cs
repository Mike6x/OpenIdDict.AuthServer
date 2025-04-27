using Identity.Application.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Users.Endpoints;
public static class GetUserByPhoneNumberEndpoint
{
    internal static RouteHandlerBuilder MapGetUserByPhoneNumberEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("phoneNumber/{phoneNumber}/", (string phoneNumber, IUserService service) =>
        {
            return service.GetByPhoneAsync(phoneNumber, CancellationToken.None);
        })
        .WithName(nameof(GetUserByPhoneNumberEndpoint))
        .WithSummary("Get user profile by Phone Number")
        // .RequirePermission("Permissions.Handlers.View")
        .WithDescription("Get another user's profile details by phone number.");
    }
}
