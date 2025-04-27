using Framework.Core.Exceptions;
using Identity.Application.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Users.Endpoints.Account;

public static class HasPasswordEndpoint
{
    internal static RouteHandlerBuilder MapHasPasswordEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/haspassword", 
                (HttpContext httpContext, IUserService service,CancellationToken cancellationToken) =>
            {
                if (httpContext.User.Identity?.IsAuthenticated != true)
                {
                    throw new UnauthorizedException();
                }
                
                return service.HasPasswordAsync(httpContext, cancellationToken);
            })
                    .WithName(nameof(HasPasswordEndpoint))
                    .WithSummary("Check if  current claim has password")
                    // .RequirePermission("Permissions.Handlers.Remove")
                    .WithDescription("Check if current user has password");
    }
}