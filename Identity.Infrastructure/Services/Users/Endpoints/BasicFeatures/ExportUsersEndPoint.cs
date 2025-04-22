using Identity.Application.Users.Abstractions;
using Identity.Application.Users.Features.ExportUsers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Users.Endpoints
{
    public static class ExportUsersEndpoint
    {
        internal static RouteHandlerBuilder MapExportUsersEndpoint(this IEndpointRouteBuilder endpoints)
        {
            return endpoints.MapPost("/export", (ExportUsersRequest request, IUserService service, CancellationToken cancellationToken) =>
            {
                return service.ExportAsync(request, cancellationToken);
            })
            .WithName(nameof(ExportUsersEndpoint))
            .WithSummary("Export a list of users with paging support")
            // .RequirePermission("Permissions.Endpoints.Export")
            .WithDescription("Export a list of users with paging support");
        }
    }
}
