using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Authenticator;

public static class Extensions
{
    public static IEndpointRouteBuilder MapAuthenticatorEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapIsAuthenticatorEnabledEndpoint();
        app.MapGetAuthenticatorAndUriEndpoint();
        app.MapEnableAuthenticatorEndpoint();
        app.MapDisableAuthenticatorEndpoint();
        app.MapResetAuthenticatorEndpoint();
        app.MapGenerateRecoverCodesEndpoint();
        app.MapCountActiveRecoveryCodesEndpoint();

        return app;
    }
}
