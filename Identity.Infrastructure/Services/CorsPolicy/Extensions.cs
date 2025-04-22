using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.CorsPolicy.Handlers;

public static class Extensions
{
    public static IEndpointRouteBuilder MapCorsPolicyEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetOrigins();
        app.MapRefreshOrigins();
        app.MapAddOrigins();
        app.MapRemoveOrigins();

        return app;
    }
}
