using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Scopes;

public static class Extensions
{
    public static IEndpointRouteBuilder MapScopeEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCreateScopeEndpoint();
        app.MapGetScopeEndpoint();
        app.MapGetScopesEndpoint();
        app.MapSearchScopesEndpoint();
        app.MapDeleteScopeEndpoint();
        app.MapUpdateScopeEndpoint();

        return app;
    }
}
