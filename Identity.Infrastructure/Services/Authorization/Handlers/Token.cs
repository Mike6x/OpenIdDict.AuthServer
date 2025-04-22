using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace Identity.Infrastructure.Services.Authorization.Endpoints;

public static class Token
{
    public static async Task<IResult> Handler(HttpContext httpContext)
    {
        var request = httpContext.GetOpenIddictServerRequest()??
                      throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");
        
        var result = await httpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        var principal = result.Principal;
        if (
            !(
                request.IsAuthorizationCodeGrantType() ||
                request.IsRefreshTokenGrantType()
            ))
        {
            principal = null;
        }

        if (principal != null)
            return Results.SignIn(principal, authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        return Results.Forbid(authenticationSchemes: new List<string>
        {
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
        });

    }
    
}