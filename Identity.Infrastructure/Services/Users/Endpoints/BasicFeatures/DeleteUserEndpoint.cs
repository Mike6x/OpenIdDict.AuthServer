using Identity.Application.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Users.Endpoints.BasicFeatures;
public static class DeleteUserEndpoint
{
    internal static RouteHandlerBuilder MapDeleteUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapDelete("/{userId:guid}", (string userId, IUserService service) 
                => service.DeleteAsync(userId))
        .WithName(nameof(DeleteUserEndpoint))
        .WithSummary("delete a user")
        // .RequirePermission("Permissions.Users.Remove")
        .WithDescription("delete a user");
    }
}
