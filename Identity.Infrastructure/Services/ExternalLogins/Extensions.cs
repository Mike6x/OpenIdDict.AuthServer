using Identity.Infrastructure.Services.ExternalLogins.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.ExternalLogins;

public static class Extensions
{
    public static IEndpointRouteBuilder MapExternalLoginEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetExternalLoginsEndpoint();
        app.MapDeleteExternalLoginEndpoint();

        app.MapGetExternalCallbackEndpoint();
        app.MapExternalCallbackEndpoint();
        
        return app;
    }
}
