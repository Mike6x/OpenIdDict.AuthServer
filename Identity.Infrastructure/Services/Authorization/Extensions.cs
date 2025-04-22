using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Authorization.Endpoints;

public static class Extensions
{
    public static IEndpointRouteBuilder MapOpenIdDictEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetAuthorizeEndpoint();
        app.MapPostAuthorizeEndpoint();

        app.MapAcceptAuthorizationGrantEndpoint();
        app.MapDenyAuthorizationGrantEndpoint();

        app.MapGetVerifyConnectionEndpoint();
        app.MapPostVerifyConnectionEndpoint();
        app.MapDenyVerifyConnectionEndpoint();
        
        app.MapGetEndSessionEndpoint();
        app.MapPostEndSessionEndpoint();
        
        app.MapGetTokenEndpoint();
        app.MapPostTokenEndpoint();
                
        app.MapGetUserInfoEndpoint();
        app.MapPostUserInfoEndpoint();
        
        return app;
    }
}