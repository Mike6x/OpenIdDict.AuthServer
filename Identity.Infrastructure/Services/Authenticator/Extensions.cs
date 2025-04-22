using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Authenticator.Endpoints;

public static class Extensions
{
    public static IEndpointRouteBuilder MapAuthenticatorEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapIsAuthenticatorEnabledEndpoint();
        app.MapGetAuthenticatorAndUriEndpoint();
        app.MapPostEnableAuthenticatorEndpoint();
        app.MapPostDisableAuthenticatorEndpoint();
        app.MapPostResetAuthenticatorEndpoint();
        app.MapGenerateRecoverCodesEndpoint();
        app.MapCountActiveRecoveryCodesEndpoint();

        return app;
    }
}
