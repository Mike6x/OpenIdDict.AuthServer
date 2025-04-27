using Identity.Infrastructure.Services.ExternalLogins.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.ExternalLogins;

public static class GetExternalLoginsEndpoint
{
    public static RouteHandlerBuilder MapGetExternalLoginsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/", GetExternalLogins.Handler)
            .WithName(nameof(GetExternalLoginsEndpoint))
            .RequireAuthorization()
            .WithSummary("Get the external logins.")
            .WithDescription("Get the external logins for user account");
    }
}

public static class DeleteExternalLoginEndpoint
{
    public static RouteHandlerBuilder MapDeleteExternalLoginEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapDelete("/{loginProvider}/{providerKey}", DeleteExternalLogin.Handler)
            .WithName(nameof(DeleteExternalLoginEndpoint))
            .WithSummary("Remove external login.")
            .RequireAuthorization()
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Remove external login from user account.");
    }
}

//https://github.com/andyrub18/AuthServer/blob/main/AuthServer/Endpoints/ExternalCallbackEndpoint.cs

public static class GetExternalCallbackEndpoint
{
    public static RouteHandlerBuilder MapGetExternalCallbackEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/signin-google", ExternalLogin.Handler)
            .WithName(nameof(GetExternalCallbackEndpoint))
            ;
    }
}

public static class ExternalCallbackEndpoint
{
    public static RouteHandlerBuilder MapExternalCallbackEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/signin-google", ExternalLogin.Handler)
            .WithName(nameof(ExternalCallbackEndpoint));
    }
    
}