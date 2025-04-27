using System.Security.Claims;
using Framework.Core.Exceptions;
using Framework.Infrastructure.Common.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace Identity.Infrastructure.Services.Authorization;

public partial class OpenIdDictService
{
    // Note: to support non-interactive flows like password,
    // you must provide your own token endpoint action:
    #region Password, authorization code, device and refresh token flows
    
    public  async Task<IResult> ExchangeAsync(HttpContext httpContext)
    {
        var request = httpContext.GetOpenIddictServerRequest() 
                      ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");
        
        if (request.IsClientCredentialsGrantType()) return await HandleExchangeClientCredentialsGrantType(request);
        
        if (request.IsPasswordGrantType()) return await HandleExchangePasswordGrantType(request);
        
        if (request.IsAuthorizationCodeGrantType() || request.IsDeviceCodeGrantType() || request.IsRefreshTokenGrantType())
        {
            return await HandleExchangeAuthorizationAndDeviceCodeAndRefreshTokenReGrantType(httpContext);
        }

        throw new NotImplementedException("The specified grant type is not supported.");
    }
    
    private async Task<IResult> HandleExchangeAuthorizationAndDeviceCodeAndRefreshTokenReGrantType(HttpContext httpContext)
    {
        // Retrieve the claims principal stored in the authorization code/refresh token.
        var result = await httpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        
        // Retrieve the user profile corresponding to the authorization code/refresh token.
        var user = await userManager.FindByIdAsync(result.Principal?.GetClaim(OpenIddictConstants.Claims.Subject)?? string.Empty);
        if (user is null)
        {
            return Results.Forbid(
                properties: new AuthenticationProperties(new Dictionary<string, string>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The token is no longer valid."
                }!),
                authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme]);
        }

        // Ensure the user is still allowed to sign in.
        if (!await signInManager.CanSignInAsync(user))
        {
            return Results.Forbid( 
                properties: new AuthenticationProperties(new Dictionary<string, string>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is no longer allowed to sign in."
                }!),
                authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme]);
        }
        
        // Create a new ClaimsPrincipal
        var identity = new ClaimsIdentity(result.Principal?.Claims,
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: OpenIddictConstants.Claims.Name,
            roleType: OpenIddictConstants.Claims.Role);

        // Override the user claims present in the principal in case they
        // changed since the authorization code/refresh token was issued.
        identity.SetClaim(OpenIddictConstants.Claims.Subject, await userManager.GetUserIdAsync(user))
            .SetClaim(OpenIddictConstants.Claims.Email, await userManager.GetEmailAsync(user))
            .SetClaim(OpenIddictConstants.Claims.Name, await userManager.GetUserNameAsync(user))
            .SetClaim(OpenIddictConstants.Claims.PreferredUsername, await userManager.GetUserNameAsync(user))
            .SetClaims(OpenIddictConstants.Claims.Role, [..(await userManager.GetRolesAsync(user))]);
       
        await AddUserClaimsAsync(identity, user);
        
        identity.SetDestinations(GetDestinations);

        // Returning a SignInResult will ask Authorization to issue the appropriate access/identity tokens.
        return Results.SignIn(new ClaimsPrincipal(identity), authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
    
    private async Task<IResult> HandleExchangePasswordGrantType( OpenIddictRequest request)
    {
        //var user = await userManager.FindByNameAsync(request.Username ?? throw new InvalidOperationException())
        
        if (string.IsNullOrWhiteSpace(request.Username)) throw new BadRequestException("Email is required");
        var user = request.Username.IsValidEmail()
            ? await userManager.FindByEmailAsync(request.Username.Trim().Normalize())
            : await userManager.FindByNameAsync(request.Username);
        
        if (user is null)
        {
            var properties = new AuthenticationProperties(new Dictionary<string, string>
            {
                [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "User does not exist."
            }!);

            return Results.Forbid( 
                properties: properties, 
                authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme]);
        }

        // Validate the username/password parameters and ensure the account is not locked out.
        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password ?? string.Empty, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            var errorString = result switch
            {
                { IsNotAllowed: true } => "User not allowed to login. Please confirm your email",
                { RequiresTwoFactor: true } => "User requires 2F authentication",
                { IsLockedOut: true } => "User has been temporarily locked due to multiple unsuccessful login attempts.",
                _ => "The username/password couple is invalid."
            };
            
            var properties = new AuthenticationProperties(new Dictionary<string, string>
            {
                [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = errorString
            }!);
            
            return Results.Forbid( 
                properties: properties, 
                authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme]);
        }

        // Create the claims-based identity that will be used by Authorization to generate tokens.
        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: OpenIddictConstants.Claims.Name,
            roleType: OpenIddictConstants.Claims.Role);

        // Add the claims that will be persisted in the tokens.
        identity.SetClaim(OpenIddictConstants.Claims.Subject, await userManager.GetUserIdAsync(user))
                .SetClaim(OpenIddictConstants.Claims.Email, await userManager.GetEmailAsync(user))
                .SetClaim(OpenIddictConstants.Claims.Name, await userManager.GetUserNameAsync(user))
                .SetClaims(OpenIddictConstants.Claims.Role, [..(await userManager.GetRolesAsync(user))]);

        await AddUserClaimsAsync(identity, user);

        // Set the list of scopes granted to the client application.
        identity.SetScopes(new[]
        {
            OpenIddictConstants.Scopes.OpenId,
            OpenIddictConstants.Scopes.Email,
            OpenIddictConstants.Scopes.Profile,
            OpenIddictConstants.Scopes.Roles
        }.Intersect(request.GetScopes()));

        identity.SetDestinations(GetDestinations);
        
        return Results.SignIn(new ClaimsPrincipal(identity), authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
    
    private async Task<IResult> HandleExchangeClientCredentialsGrantType(OpenIddictRequest request)
    {
        // Note: the client credentials are automatically validated by Authorization:
        // if client_id or client_secret are invalid, this action won't be invoked.
        var application =
            await applicationManager.FindByClientIdAsync(request.ClientId ?? throw new InvalidOperationException()) 
            ?? throw new InvalidOperationException("The application cannot be found.");

        // Create the claims-based identity that will be used by Authorization to generate tokens.
        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: OpenIddictConstants.Claims.Name,
            roleType: OpenIddictConstants.Claims.Role);

        // Add the claims that will be persisted in the tokens (use the client_id as the subject identifier).
        identity.SetClaim(OpenIddictConstants.Claims.Subject, await applicationManager.GetClientIdAsync(application));
        identity.SetClaim(OpenIddictConstants.Claims.Name, await applicationManager.GetDisplayNameAsync(application));

        // Note: In the original OAuth 2.0 specification, the client credentials grant
        // doesn't return an identity token, which is an OpenID Connect concept.
        //
        // As a non-standardized extension, Authorization allows returning an id_token
        // to convey information about the client application when the "openid" scope
        // is granted (i.e specified when calling principal.SetScopes()). When the "openid"
        // scope is not explicitly set, no identity token is returned to the client application.

        // Set the list of scopes granted to the client application in access_token.
        identity.SetScopes(request.GetScopes());
        identity.SetResources(await scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());
        identity.SetDestinations(GetDestinations);
        
        return Results.SignIn(new ClaimsPrincipal(identity), authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
    
    #endregion
}

