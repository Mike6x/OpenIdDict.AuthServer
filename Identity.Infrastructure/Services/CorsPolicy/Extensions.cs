using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.CorsPolicy;

public static class Extensions
{
    public static IEndpointRouteBuilder MapCorsPolicyEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetOriginsEndpoint();
        app.MapRefreshOriginsEndpoint();
        app.MapAddOriginsEndpoint();
        app.MapRemoveOriginsEndpoint();

        return app;
    }
}
