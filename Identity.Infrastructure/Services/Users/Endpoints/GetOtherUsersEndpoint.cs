using Identity.Application.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Users.Endpoints
{
    public static class  GetOtherUsersEndpoint
    {
        internal static RouteHandlerBuilder MapGetOtherUsersEndpoint(this IEndpointRouteBuilder endpoints)
        {
            return endpoints.MapGet("{userId}/otherusers", async (string userId,IUserService service, CancellationToken cancellationToken) =>
            {
                var list = await service.GetAllAsync(cancellationToken);

                return list.Where(user => user.Id.ToString() != userId).ToList();
            })
            .WithName(nameof(GetOtherUsersEndpoint))
            .WithSummary("get others")
            // .RequirePermission("Permissions.Handlers.Search")
            .WithDescription("Get list of other users");
        }
        
    }
}
