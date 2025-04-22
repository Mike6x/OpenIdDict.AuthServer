using Identity.Application.Users.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Users.Endpoints.BasicFeatures;
public static class GetUsersEndpoint
{
    internal static RouteHandlerBuilder MapGetUsersEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/", (CancellationToken cancellationToken, IUserService service) =>
        {
            return service.GetAllAsync(cancellationToken);
        })
        .WithName(nameof(GetUsersEndpoint))
        .WithSummary("get all users ")
        // .RequirePermission("Permissions.Users.Search")
        .WithDescription("get all users ");
    }
}
