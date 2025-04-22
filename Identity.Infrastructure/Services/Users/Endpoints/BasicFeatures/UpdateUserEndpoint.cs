using Identity.Application.Users.Abstractions;
using Identity.Application.Users.Features.UpdateUser;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Users.Endpoints;
public static class UpdateUserEndpoint
{
    internal static RouteHandlerBuilder MapUpdateUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPut("/", (
            string id,
            UpdateUserCommand request,       
            HttpContext context,
            IUserService service) =>
        {
            
            var origin = $"{context.Request.Scheme}://{context.Request.Host.Value}{context.Request.PathBase.Value}";

            return service.UpdateProfileAsync(request, id, origin);
        })
        .WithName(nameof(UpdateUserEndpoint))
        .WithSummary("update user profile")
        // .RequirePermission("Permissions.Endpoints.Update")
        .WithDescription("update user profile");
    }
}
