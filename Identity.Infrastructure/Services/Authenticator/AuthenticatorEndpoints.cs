using Identity.Infrastructure.Services.Authenticator.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Authenticator;

public static class IsAuthenticatorEnabledEndpoint
{
    public static RouteHandlerBuilder MapIsAuthenticatorEnabledEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/isenabled", IsAuthenticatorEnabled.Handler)
            .WithName(nameof(IsAuthenticatorEnabledEndpoint))
            .WithSummary("Is enabled")
            .WithDescription("Gets whether authenticator is enabled for the user account");
    }
}

public static class GetAuthenticatorAndUriEndpoint
{
    public static RouteHandlerBuilder MapGetAuthenticatorAndUriEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/", GetAuthenticatorAndUri.Handler)
            .WithName(nameof(GetAuthenticatorAndUriEndpoint))
            .WithSummary("Get User Authenticator And Uri")
            .WithDescription("Get the details required for setting up authenticator for the user");
    }
}

public static class EnableAuthenticatorEndpoint
{
    public static RouteHandlerBuilder MapEnableAuthenticatorEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/enable", SetUser2FaStatus.EnableHandler)
                .WithName(nameof(EnableAuthenticatorEndpoint))
                .WithSummary("Enable the authenticator or user account")
                .WithDescription("Enable the authenticator by verifying the provided Code from authenticator app.");
    }
}

public static class DisableAuthenticatorEndpoint
{
    public static RouteHandlerBuilder MapDisableAuthenticatorEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/disable", SetUser2FaStatus.DisableHandler)
                .WithName(nameof(DisableAuthenticatorEndpoint))
                .WithSummary("Disable authenticator for the user account")
                .WithDescription("Disable authenticator for the user account.");
    }
}

public static class ResetAuthenticatorEndpoint
{
    public static RouteHandlerBuilder MapResetAuthenticatorEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/reset", SetUser2FaStatus.ResetHandler )
            .WithName(nameof(ResetAuthenticatorEndpoint))
            .WithSummary("Reset authenticator for the user account")
            .WithDescription("Reset authenticator for the user account.");
    }
}

public static class GenerateRecoverCodesEndpoint
{
    public static RouteHandlerBuilder MapGenerateRecoverCodesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/recoverycodes", GenerateRecoverCodes.Handler)
            .WithName(nameof(GenerateRecoverCodesEndpoint))
            .WithSummary("Generate new recovery codes")
            .WithDescription("Generate new recovery codes for the authenticator");
    }
}

public static class CountActiveRecoveryCodesEndpoint
{
    public static RouteHandlerBuilder MapCountActiveRecoveryCodesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/recoverycodescount", CountActiveRecoveryCodes.Handler)
            .WithName(nameof(CountActiveRecoveryCodesEndpoint))
            .WithSummary("Count Active recovery codes")
            .WithDescription(" Get the number of remaining recovery codes ");
    }
}