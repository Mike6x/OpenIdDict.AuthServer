using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Clients;

public static class Extensions
{
    public static IEndpointRouteBuilder MapApplicationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCreateApplicationEndpoint();
        app.MapGetApplicationEndpoint();
        app.MapGetApplicationsEndpoint();
        app.MapSearchApplicationsEndpoint();
        
        app.MapUpdateApplicationEndpoint();
        app.MapDeleteApplicationEndpoint();
        
        app.MapCallBackApplicationEndpoint();

        return app;
    }
}
