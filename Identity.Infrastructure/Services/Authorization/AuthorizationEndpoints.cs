using Identity.Application.Authorization;
using Identity.Infrastructure.Services.Authorization.Endpoints;
using Identity.Infrastructure.Services.Authorization.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OpenIddict.Server.AspNetCore;

namespace Identity.Infrastructure.Services.Authorization;

#region Authorization code, implicit and hybrid flows
// Note: to support interactive flows like the code flow,
// you must provide your own authorization endpoint action:
public static class GetAuthorizeEndpoint
{
    public static RouteHandlerBuilder MapGetAuthorizeEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/authorize", Authorize.Handler)
            .WithName(nameof(GetAuthorizeEndpoint))
            .WithSummary("Get Authorize Information")
            .WithDescription("Retrieve Authorize Information.")
            .AllowAnonymous()
            .DisableAntiforgery();
    }
}

public static class AuthorizeEndpoint
{
    public static RouteHandlerBuilder MapAuthorizeEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/authorize", (HttpContext httpContext, IAuthorizationService service, CancellationToken cancellationToken) 
                => service.AuthorizeAsync(httpContext))
            .WithName(nameof(AuthorizeEndpoint))
            .WithSummary("Retrieve Authorize Information")
            .WithDescription("Retrieve Authorize Information.")
            .AllowAnonymous()
            .DisableAntiforgery();
    }
}
public static class AcceptAuthorizationGrantEndpoint
{
    public static RouteHandlerBuilder MapAcceptAuthorizationGrantEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/authorize/accept", (HttpContext httpContext, IAuthorizationService service, CancellationToken cancellationToken) 
                => service.AcceptAsync(httpContext))
            .WithName(nameof(AcceptAuthorizationGrantEndpoint))
            .WithSummary("Deny authorization grant")
            .WithDescription("Deny authorization grant")
            .RequireAuthorization();
    }
}

// Notify Authorization that the authorization grant has been denied by the resource owner
// to redirect the user agent to the client application using the appropriate response_mode.
public static class DenyAuthorizationGrantEndpoint
{
    public static RouteHandlerBuilder MapDenyAuthorizationGrantEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/authorize/deny", (IAuthorizationService service, CancellationToken cancellationToken) 
                => service.Deny())
            .WithName(nameof(DenyAuthorizationGrantEndpoint))
            .WithSummary("Deny authorization grant")
            .WithDescription("Deny authorization grant")
            .RequireAuthorization();
    }
}


#endregion

#region Device flow
// Note: to support the device flow, you must provide your own verification endpoint action:

public static class GetVerifyConnectionEndpoint
{
    public static RouteHandlerBuilder MapGetVerifyConnectionEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/verify",(HttpContext httpContext, IAuthorizationService service, CancellationToken cancellationToken) 
                => service.VerifyAsync(httpContext))
            .WithName(nameof(GetVerifyConnectionEndpoint))
            .WithSummary("Verify connection")
            .WithDescription("Verify connection")
            .RequireAuthorization();
    }
}
public static class VerifyConnectionEndpoint
{
    public static RouteHandlerBuilder MapVerifyConnectionEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/verify",(HttpContext httpContext, IAuthorizationService service, CancellationToken cancellationToken) 
                => service.VerifyAcceptAsync(httpContext))
            .WithName(nameof(VerifyConnectionEndpoint))
            .WithSummary("Accept verify connection ")
            .WithDescription("Accept verify connection ")
            .RequireAuthorization();
    }
}

public static class DenyVerifyConnectionEndpoint
{
    public static RouteHandlerBuilder MapDenyVerifyConnectionEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/verify/deny",(IAuthorizationService service, CancellationToken cancellationToken) 
                => service.VerifyDeny())
            .WithName(nameof(DenyVerifyConnectionEndpoint))
            .WithSummary("Deny verify connection ")
            .WithDescription("Deny verify connection ")
            .RequireAuthorization();
    }
}

#endregion

#region  Logout support for interactive flows like code and implicit
// Note: the logout action is only useful when implementing interactive
// flows like the authorization code flow or the implicit flow.

public static class GetEndSessionEndpoint
{
    public static RouteHandlerBuilder MapGetEndSessionEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/endsession", () => Results.Ok())
            .WithName(nameof(GetEndSessionEndpoint))
            .WithSummary("Terminate Session")
            .WithDescription("Terminate the session.");
    }
}

public static class EndSessionEndpoint
{
    public static RouteHandlerBuilder MapEndSessionEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/endsession", (IAuthorizationService service, CancellationToken cancellationToken) 
                => service.EndSessionAsync())
            .WithName(nameof(EndSessionEndpoint))
            .WithSummary("Terminate Session")
            .WithDescription("Terminate the session.");
    }
}

#endregion

#region Password, authorization code, device and refresh token flows.
// Note: to support non-interactive flows like password,
// you must provide your own token endpoint action:

public static class GetTokenEndpoint
{
    public static RouteHandlerBuilder MapGetTokenEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/token", (Delegate)Token.Handler)
            .WithName(nameof(GetTokenEndpoint))
            .WithSummary("Retrieve Access Token.")
            .WithDescription("Retrieve Access Token.")
            .AllowAnonymous()
            .DisableAntiforgery<RouteHandlerBuilder>();
    }
}

public static class TokenEndpoint
{
    public static RouteHandlerBuilder MapTokenEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/token", (HttpContext httpContext, IAuthorizationService service, CancellationToken cancellationToken) 
                => service.ExchangeAsync(httpContext))
            .WithName(nameof(TokenEndpoint))
            .WithSummary("Retrieve Access Token.")
            .WithDescription("Retrieve Access Token.")
            .AllowAnonymous()
            .DisableAntiforgery<RouteHandlerBuilder>();
    }
}

#endregion

#region User Infomation

public static class GetUserInfoEndpoint
{
    public static RouteHandlerBuilder MapGetUserInfoEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/userinfo", UserInfo.Handler)
            .WithName(nameof(GetUserInfoEndpoint))
            .WithSummary("Retrieve User Info.")
            .WithDescription("Retrieve User Info.")
            //.Produces("application/json")
            .RequireAuthorization(policy =>
                {
                    policy.AddAuthenticationSchemes(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                }
            );
    }
}

public static class UserInfoEndpoint
{
    public static RouteHandlerBuilder MapUserInfoEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/userinfo", UserInfo.Handler)
            .WithName(nameof(UserInfoEndpoint))
            .WithSummary("Retrieve User Info.")
            .WithDescription("Retrieve User Info..")
            //.Produces("application/json")
            .RequireAuthorization(policy =>
                {
                    policy.AddAuthenticationSchemes(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                }
            );
    }
}

#endregion