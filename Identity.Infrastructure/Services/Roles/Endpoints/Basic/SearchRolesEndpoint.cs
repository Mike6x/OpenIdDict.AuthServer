using Identity.Application.Roles;
using Identity.Application.Roles.Features.SearchRoles;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Roles.Endpoints.Basic;

public static class SearchRolesEndpoint
{
    internal static RouteHandlerBuilder MapSearchRolesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/search", (SearchRolesRequest request, IRoleService service, CancellationToken cancellationToken) =>
            {
                return service.SearchAsync(request, cancellationToken);
            })
            .WithName(nameof(SearchRolesEndpoint))
            .WithSummary("get a list of roles with paging support")
            // .RequirePermission("Permissions.Roles.Search")
            .WithDescription("get a list of roles with paging support");
    }
}