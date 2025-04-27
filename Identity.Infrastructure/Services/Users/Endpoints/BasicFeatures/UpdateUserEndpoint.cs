using Identity.Application.Users;
using Identity.Application.Users.Features.UpdateUser;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Users.Endpoints.BasicFeatures;
public static class UpdateUserEndpoint
{
    internal static RouteHandlerBuilder MapUpdateUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPut("/", ( HttpContext context,
            string id,
            UpdateUserCommand request,   
            IUserService service,
            CancellationToken cancellationToken) =>
        {
            
            var origin = $"{context.Request.Scheme}://{context.Request.Host.Value}{context.Request.PathBase.Value}";

            return service.UpdateProfileAsync(request, id, origin, cancellationToken);
        })
        .WithName(nameof(UpdateUserEndpoint))
        .WithSummary("update user profile")
        // .RequirePermission("Permissions.Handlers.Update")
        .WithDescription("update user profile");
    }
}
