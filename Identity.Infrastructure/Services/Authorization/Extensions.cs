using Identity.Infrastructure.Services.Authorization.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Authorization;

public static class Extensions
{
    public static IEndpointRouteBuilder MapOpenIdDictEndpoints(this IEndpointRouteBuilder app)
    {
        var openIdConnectGroup = app.MapGroup("connect").WithTags("Authorization").WithOpenApi();
        
        openIdConnectGroup.MapGetAuthorizeEndpoint();
        openIdConnectGroup.MapAuthorizeEndpoint();

        openIdConnectGroup.MapAcceptAuthorizationGrantEndpoint();
        openIdConnectGroup.MapDenyAuthorizationGrantEndpoint();

        openIdConnectGroup.MapGetVerifyConnectionEndpoint();
        openIdConnectGroup.MapVerifyConnectionEndpoint();
        openIdConnectGroup.MapDenyVerifyConnectionEndpoint();
        
        openIdConnectGroup.MapGetEndSessionEndpoint();
        openIdConnectGroup.MapEndSessionEndpoint();
        
        openIdConnectGroup.MapGetTokenEndpoint();
        openIdConnectGroup.MapTokenEndpoint();
                
        openIdConnectGroup.MapGetUserInfoEndpoint();
        openIdConnectGroup.MapUserInfoEndpoint();
        
        return app;
    }
}