using Framework.Core.Exceptions;
using Identity.Application.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Users.Endpoints.CurrentUser;


public static class GetMeEndpoint
{
    internal static RouteHandlerBuilder MapGetMeEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/me", async (HttpContext httpContext, IUserService service, CancellationToken cancellationToken) =>
            {
                if (httpContext.User?.Identity?.IsAuthenticated != true)
                {
                    throw new UnauthorizedException();
                }
                return await service.GetMeAsync(httpContext, cancellationToken);
            })
            .WithName(nameof(GetMeEndpoint))
            .WithSummary("Get current user information based on token")
            .WithDescription("Get current user information based on token");
    }
}