using Identity.Domain.Entities;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace Identity.Infrastructure.Services.Authorization.Endpoints;

public static class UserInfo
{
    public static async Task<IResult> Handler(HttpContext httpContext, UserManager<AppUser> userManager)
    {
        //https://github.com/openiddict/openiddict-samples/blob/dev/samples/Dantooine/Dantooine.Server/Controllers/UserinfoController.cs
        var user = httpContext.User;
        
        _ = httpContext.GetOpenIddictServerRequest() 
            ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        var result = await httpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        var principal = result.Principal;
        if (principal == null)  return Results.Unauthorized();
        
        var loggedInUser = await userManager.GetUserAsync(principal);
        if (loggedInUser is null) 
            return Results.Challenge(
                authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
                properties: new AuthenticationProperties(new Dictionary<string, string>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidToken,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                        "The specified access token is bound to an account that no longer exists."
                }!));
        
        var claims = new Dictionary<string, object>(StringComparer.Ordinal)
        {
            // Note: the "sub" claim is a mandatory claim and must be included in the JSON response.
            [OpenIddictConstants.Claims.Subject] = await userManager.GetUserIdAsync(loggedInUser)
        };

        if (user.HasScope(OpenIddictConstants.Scopes.Email))
        {
            claims[OpenIddictConstants.Claims.Email] = await userManager.GetEmailAsync(loggedInUser) ?? string.Empty;
            claims[OpenIddictConstants.Claims.EmailVerified] = await userManager.IsEmailConfirmedAsync(loggedInUser);
        }

        if (user.HasScope(OpenIddictConstants.Scopes.Phone))
        {
            claims[OpenIddictConstants.Claims.PhoneNumber] = await userManager.GetPhoneNumberAsync(loggedInUser) ?? string.Empty;
            claims[OpenIddictConstants.Claims.PhoneNumberVerified] = await userManager.IsPhoneNumberConfirmedAsync(loggedInUser);
        }

        if (user.HasScope(OpenIddictConstants.Scopes.Roles))
        {
            claims[OpenIddictConstants.Claims.Role] = await userManager.GetRolesAsync(loggedInUser);
        }
        // Note: the complete list of standard claims supported by the OpenID Connect specification
        // can be found here: http://openid.net/specs/openid-connect-core-1_0.html#StandardClaims

        return Results.Ok(claims);
    }
        
}