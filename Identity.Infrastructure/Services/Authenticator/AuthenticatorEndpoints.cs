using Identity.Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Authenticator.Endpoints;

public static class AuthenticatorEndpoints
{
    public static RouteHandlerBuilder MapIsAuthenticatorEnabledEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/isenabled", IsAuthenticatorEnabled.Handler)
            .WithName(nameof(MapIsAuthenticatorEnabledEndpoint))
            .WithSummary("Is enabled")
            .WithDescription("Gets whether authenticator is enabled for the user account");
    }
        
    public static RouteHandlerBuilder MapGetAuthenticatorAndUriEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/", GetAuthenticatorAndUri.Handler)
            .WithName(nameof(MapGetAuthenticatorAndUriEndpoint))
            .WithSummary("Get User Authenticator And Uri")
            .WithDescription("Get the details required for setting up authenticator for the user");
    }

    public static RouteHandlerBuilder MapPostEnableAuthenticatorEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/enable", SetUser2FaStatus.EnableHandler)
                .WithName(nameof(MapPostEnableAuthenticatorEndpoint))
                .WithSummary("Enable the authenticator or user account")
                .WithDescription("Enable the authenticator by verifying the provided Code from authenticator app.");
    }

    public static RouteHandlerBuilder MapPostDisableAuthenticatorEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/disable", SetUser2FaStatus.DisableHandler)
                .WithName(nameof(MapPostDisableAuthenticatorEndpoint))
                .WithSummary("Disable authenticator for the user account")
                .WithDescription("Disable authenticator for the user account.")
            ;
    }

    public static RouteHandlerBuilder MapPostResetAuthenticatorEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/reset", SetUser2FaStatus.ResetHandler )
                .WithName(nameof(MapPostResetAuthenticatorEndpoint))
                .WithSummary("Reset authenticator for the user account")
                .WithDescription("Reset authenticator for the user account.");
    }

    public static RouteHandlerBuilder MapGenerateRecoverCodesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/recoverycodes", GenerateRecoverCodes.Handler)
            .WithName(nameof(MapGenerateRecoverCodesEndpoint))
            .WithSummary("Generate new recovery codes")
            .WithDescription("Generate new recovery codes for the authenticator");
    }

    public static RouteHandlerBuilder MapCountActiveRecoveryCodesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/recoverycodescount", CountActiveRecoveryCodes.Handler)
            .WithName(nameof(MapCountActiveRecoveryCodesEndpoint))
            .WithSummary("Count Active recovery codes")
            .WithDescription(" Get the number of remaining recovery codes ");
    }

}