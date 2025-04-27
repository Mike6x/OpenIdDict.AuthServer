using Identity.Application.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Users.Endpoints.BasicFeatures;
public static class GetUserStatisticsEndpoint
{
    internal static RouteHandlerBuilder MapGetUserStatisticsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/Statistics", (CancellationToken cancellationToken, IUserService service) 
                => service.GetUserStatisticsAsync(cancellationToken))
        .WithName(nameof(GetUserStatisticsEndpoint))
        .WithSummary("get statistics about users in the system")
        // .RequirePermission("Permissions.Users.Search")
        .WithDescription("get statistics about users in the system");
    }
}
