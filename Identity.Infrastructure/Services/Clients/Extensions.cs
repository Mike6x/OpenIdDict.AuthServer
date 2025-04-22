using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Clients.Endpoints;

public static class Extensions
{
    public static IEndpointRouteBuilder MapApplicationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCreateApplicationEndpoint();
        app.MapGetApplicationEndpoint();
        app.MapGetApplicationsEndpoint();
        app.MapSearchApplicationsEndpoint();
        app.MapDeleteApplicationEndpoint();
        app.MapUpdateApplicationEndpoint();

        app.MapCallBackApplicationEndpoint();

        return app;
    }
}
