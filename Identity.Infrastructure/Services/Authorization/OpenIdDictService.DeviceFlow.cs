using System.Collections.Immutable;
using System.Security.Claims;
using Framework.Infrastructure.Common.Extensions;
using Identity.Application.Authorization.ViewModels;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace Identity.Infrastructure.Services.Authorization;

public partial class OpenIdDictService
{
    #region Device flow
    
    // Note: to support the device flow, you must provide your own verification endpoint action:
    public async Task<IResult> VerifyAsync(HttpContext httpContext)
    {
        var request = httpContext.GetOpenIddictServerRequest() ??
                      throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        // If the user code was not specified in the query string (e.g as part of the verification_uri_complete),
        // render a form to ask the user to enter the user code manually (non-digit chars are automatically ignored).
        if (string.IsNullOrEmpty(request.UserCode))
        {
            return Results.Ok(new VerifyViewModel());
        }

        // Retrieve the claims principal associated with the user code.
        var result = await httpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        if (result.Succeeded)
        {
            // Retrieve the application details from the database using the client_id stored in the principal.
            var application = await applicationManager.FindByClientIdAsync(result.Principal.GetClaim(OpenIddictConstants.Claims.ClientId)) ??
                              throw new InvalidOperationException("Details concerning the calling client application cannot be found.");

            // Render a form asking the user to confirm the authorization demand.
            return Results.Ok(new VerifyViewModel
            {
                ApplicationName = await applicationManager.GetLocalizedDisplayNameAsync(application)?? string.Empty,
                Scope = string.Join(" ", result.Principal.GetScopes()),
                UserCode = request.UserCode
            });
        }

        // Redisplay the form when the user code is not valid.
        return Results.Ok(new VerifyViewModel
        {
            Error = OpenIddictConstants.Errors.InvalidToken,
            ErrorDescription = "The specified user code is not valid. Please make sure you typed it correctly."
        });
    }
    
    public async Task<IResult> VerifyAcceptAsync(HttpContext httpContext)
    {
        // Retrieve the claims principal associated with the user code.
        var result = await httpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        
        var principal = result.Principal;
        if (principal == null) 
            return Results.Challenge(
                authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
                properties: new AuthenticationProperties(new Dictionary<string, string>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidToken,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                        "The specified access token is bound to an account that no longer exists."
                }!));
        
        // Retrieve the profile of the logged in user.
        var loggedInUser = await userManager.GetUserAsync(principal) ??
            throw new InvalidOperationException("The user details cannot be retrieved.");

        if (result.Succeeded)
        {
            // Create the claims-based identity that will be used by Authorization to generate tokens.
            var identity = new ClaimsIdentity(
                authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                nameType: OpenIddictConstants.Claims.Name,
                roleType: OpenIddictConstants.Claims.Role);

            // Add the claims that will be persisted in the tokens.
            identity.SetClaim(OpenIddictConstants.Claims.Subject, await userManager.GetUserIdAsync(loggedInUser))
                    .SetClaim(OpenIddictConstants.Claims.Email, await userManager.GetEmailAsync(loggedInUser))
                    .SetClaim(OpenIddictConstants.Claims.Name, await userManager.GetUserNameAsync(loggedInUser))
                    .SetClaims(OpenIddictConstants.Claims.Role, (await userManager.GetRolesAsync(loggedInUser)).ToImmutableArray());

            await AddUserClaimsAsync(identity, loggedInUser);

            // Note: in this sample, the granted scopes match the requested scope
            // but you may want to allow the user to uncheck specific scopes.
            // For that, simply restrict the list of scopes before calling SetScopes.
            identity.SetScopes(result.Principal.GetScopes());
            identity.SetResources(await scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());
            identity.SetDestinations(GetDestinations);

            var properties = new AuthenticationProperties
            {
                // This property points to the address Authorization will automatically
                // redirect the user to after validating the authorization demand.
                RedirectUri = "/pauth"
            };

            return Results.SignIn(
                new ClaimsPrincipal(identity),
                properties: properties, 
                authenticationScheme:OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        // Redisplay the form when the user code is not valid.
        return Results.Ok(new VerifyViewModel
        {
            Error = OpenIddictConstants.Errors.InvalidToken,
            ErrorDescription = "The specified user code is not valid. Please make sure you typed it correctly."
        });
    }

    // Notify Authorization that the authorization grant has been denied by the resource owner.
    public IResult VerifyDeny() => Results.Forbid(
        properties: new AuthenticationProperties
        {
            // This property points to the address Authorization will automatically
            // redirect the user to after rejecting the authorization demand.
            RedirectUri = "/pauth"
        },
        authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,]);

    #endregion
}