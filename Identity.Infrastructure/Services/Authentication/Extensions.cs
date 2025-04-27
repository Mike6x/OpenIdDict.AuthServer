using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Authentication;

public static class Extensions
{
    public static IEndpointRouteBuilder MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
    {

        app.MapLogInEndpoint();
        app.MapLogOutEndpoint();
        
        app.MapGetLogInCallBackEndpoint();
        app.MapLogInCallBackEndpoint();
        
        return app;
    }
}