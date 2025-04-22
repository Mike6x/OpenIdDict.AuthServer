using Identity.Application.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Aurhentication;

public static class AuthenticationEndpoint
{
    public static RouteHandlerBuilder MapPostLogInEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/login", (LoginRequest request, IAuthenticationService service, CancellationToken cancellationToken) 
                => service.LogInAsync(request))
            .WithName(nameof(MapPostLogInEndpoint))
            .WithSummary("Login User")
            .WithDescription("Login User")
            .AllowAnonymous();
    }
    
    public static RouteHandlerBuilder MapPostLogOutEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/logout", (string? returnUrl,IAuthenticationService service, CancellationToken cancellationToken) 
                => service.LogOutAsync(returnUrl))
            .WithName(nameof(MapPostLogOutEndpoint))
            .WithSummary("Log Out")
            .WithDescription("Log Out.")
            .RequireAuthorization();
    }
    
    // Note: this controller uses the same callback action for all providers
    // but for users who prefer using a different action per provider,
    // the following action can be split into separate actions.
    public static RouteHandlerBuilder MapGetLogInCallBackEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/callback", (HttpContext httpContext, IAuthenticationService service, CancellationToken cancellationToken) 
                => service.LogInCallBackAsync(httpContext))
            .WithName(nameof(MapGetLogInCallBackEndpoint))
            .WithSummary("Login Callback")
            .WithDescription("Login Callback")
            .DisableAntiforgery();
    }
    public static RouteHandlerBuilder MapPostLogInCallBackEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/callback", (HttpContext httpContext, IAuthenticationService service, CancellationToken cancellationToken) 
                => service.LogInCallBackAsync(httpContext))
            .WithName(nameof(MapPostLogInCallBackEndpoint))
            .WithSummary("Login Callback")
            .WithDescription("Login Callback")
            .DisableAntiforgery();
    }
}