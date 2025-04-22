using Identity.Application.Users.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Users.Endpoints;
public static class DeleteUserEndpoint
{
    internal static RouteHandlerBuilder MapDeleteUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapDelete("/{id:guid}", (string id, IUserService service) =>
        {
            return service.DeleteAsync(id);
        })
        .WithName(nameof(DeleteUserEndpoint))
        .WithSummary("delete user")
        // .RequirePermission("Permissions.Users.Delete")
        .WithDescription("delete user");
    }
}
