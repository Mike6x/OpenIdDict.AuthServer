// https://github.com/andyrub18/AuthServer/blob/main/AuthServer/Endpoints/ExternalCallbackEndpoint.cs
using System.Security.Claims;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using OpenIddict.Client.AspNetCore;

namespace Identity.Infrastructure.Services.ExternalLogins.Handlers;

public static class ExternalLoginEndpoint
{
     public static async Task<IResult> Handler(
         HttpContext context, 
         UserManager<AppUser> userManager,
         SignInManager<AppUser> signInManager,
         string returnUri = "~/")
    {
        var authenticateResult =
            await context.AuthenticateAsync(OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);
        if (!authenticateResult.Succeeded) return Results.Redirect($"/Identity/Account/Login?returnUri={returnUri}");

        if (!(authenticateResult.Principal?.Identity?.IsAuthenticated ?? false))
            return Results.UnprocessableEntity("The external authorization data cannot be used for authentication.");

        var email = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email)) return Results.NotFound("Email claim not found.");

        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = CreateUser();
            if (user is null) return Results.Problem("Failed to create user");

            user.Email = email;
            user.UserName = email;

            var result = await userManager.CreateAsync(user);
            if (!result.Succeeded)
                return Results.Problem(
                    string.Join(", ", result.Errors.Select(e => e.Description)),
                    statusCode: StatusCodes.Status400BadRequest
                );
        }

        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
        identity.SetClaim(ClaimTypes.Email, email)
            .SetClaim(ClaimTypes.Name, authenticateResult.Principal.GetClaim(ClaimTypes.Name))
            .SetClaim(ClaimTypes.NameIdentifier,
                authenticateResult.Principal.GetClaim(OpenIddictConstants.Claims.Private.RegistrationId));

        var properties = new AuthenticationProperties(authenticateResult.Properties.Items)
        {
            RedirectUri = authenticateResult.Properties.RedirectUri ?? "/",
        };

        var tokensToStore = authenticateResult.Properties.GetTokens()
            .Where(token => token.Name 
                    is OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken 
                    or OpenIddictClientAspNetCoreConstants.Tokens.BackchannelIdentityToken 
                    or OpenIddictClientAspNetCoreConstants.Tokens.RefreshToken);

        properties.StoreTokens(tokensToStore);

        await signInManager.SignInAsync(user, false, CookieAuthenticationDefaults.AuthenticationScheme);

        return Results.SignIn(new(identity), properties, CookieAuthenticationDefaults.AuthenticationScheme);
    }
     
    private static AppUser? CreateUser()
    {
        try
        {
            return Activator.CreateInstance<AppUser>();
        }
        catch
        {
            return null;
        }
    }

    
}

   
