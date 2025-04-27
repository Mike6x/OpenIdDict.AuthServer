using Identity.Application.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Authentication;

public static class LogInEndpoint
{
    public static RouteHandlerBuilder MapLogInEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/login",
                (LoginRequest request, IAuthenticationService service, CancellationToken cancellationToken)
                    => service.LogInAsync(request))
            .WithName(nameof(LogInEndpoint))
            .WithSummary("Login User")
            .WithDescription("Login User")
            .AllowAnonymous();
    }
}

public static class LogOutEndpoint
{
    public static RouteHandlerBuilder MapLogOutEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/logout",
                (string? returnUrl, IAuthenticationService service, CancellationToken cancellationToken)
                    => service.LogOutAsync(returnUrl))
            .WithName(nameof(LogOutEndpoint))
            .WithSummary("Log Out")
            .WithDescription("Log Out.")
            .RequireAuthorization();
    }
}


// Note: this controller uses the same callback action for all providers
// but for users who prefer using a different action per provider,
// the following action can be split into separate actions.
public static class GetLogInCallBackEndpoint
{
    public static RouteHandlerBuilder MapGetLogInCallBackEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/callback/{provider}",
                (HttpContext httpContext, IAuthenticationService service, CancellationToken cancellationToken)
                    => service.LogInCallBackAsync(httpContext))
            .WithName(nameof(GetLogInCallBackEndpoint))
            .WithSummary("Login Callback")
            .WithDescription("Login Callback")
            .DisableAntiforgery();
    }
}

public static class LogInCallBackEndpoint
{
    public static RouteHandlerBuilder MapLogInCallBackEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/callback{provider}",
                (HttpContext httpContext, IAuthenticationService service, CancellationToken cancellationToken)
                    => service.LogInCallBackAsync(httpContext))
            .WithName(nameof(LogInCallBackEndpoint))
            .WithSummary("Login Callback")
            .WithDescription("Login Callback")
            .DisableAntiforgery();
    }
}
