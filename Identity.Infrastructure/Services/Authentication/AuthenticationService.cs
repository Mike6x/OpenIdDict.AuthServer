using System.Security.Claims;
using Framework.Core.Exceptions;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using OpenIddict.Abstractions;
using OpenIddict.Client.AspNetCore;
using OpenIddict.Server.AspNetCore;
using IAuthenticationService = Identity.Application.Authentication.IAuthenticationService;

namespace Identity.Infrastructure.Services.Aurhentication;

public class AuthenticationService(
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    IOpenIddictApplicationManager applicationManager,
    IOpenIddictScopeManager scopeManager) : IAuthenticationService
{
    
    public async Task<IResult> LogInAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email)) throw new BadRequestException("Email is required");
        
        var user = await userManager.FindByEmailAsync(request.Email) 
                   ?? throw new NotFoundException($"User with email {request.Email} not found");
        
        if (!user.IsActive) 
            throw new UnauthorizedException($"Account is disabled");
        
        if (user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow)
            throw new UnauthorizedException($"Account is locked until {user.LockoutEnd }");
        
        if (!await userManager.CheckPasswordAsync(user, request.Password)) 
            throw new UnauthorizedException("Invalid credentials");
        

        const bool isPersistent = true;

        if (string.IsNullOrWhiteSpace(user.UserName)) user.UserName = user.Email;
        
        user.LastLoginOn = DateTime.UtcNow;
        await userManager.UpdateAsync(user);
        
        var result = await signInManager.PasswordSignInAsync(user.UserName, request.Password, isPersistent, true);

        if (result.RequiresTwoFactor)
        {
            if (!string.IsNullOrEmpty(request.TwoFactorCode))
            {
                result = await signInManager.TwoFactorAuthenticatorSignInAsync(request.TwoFactorCode, isPersistent, rememberClient: isPersistent);
            }
            else if (!string.IsNullOrEmpty(request.TwoFactorRecoveryCode))
            {
                result = await signInManager.TwoFactorRecoveryCodeSignInAsync(request.TwoFactorRecoveryCode);
            }
            if (!result.Succeeded)
                return Results.Accepted("Otp Required");
        }

        return !result.Succeeded ? Results.Unauthorized() : Results.Ok("Logged In");
    }
    
    public async Task<IResult> LogOutAsync(string? returnUrl)
    {
        await signInManager.SignOutAsync();
        
        if(string.IsNullOrWhiteSpace(returnUrl)) returnUrl ="/";
        
        return Results.SignOut(
            authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
            properties: new AuthenticationProperties { RedirectUri = returnUrl });

    }
    
    public async Task<IResult> LogInCallBackAsync(HttpContext httpContext)
    {
        var result = await httpContext.AuthenticateAsync(OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);
        
        if (result.Principal is not ClaimsPrincipal { Identity.IsAuthenticated: true })
        {
            throw new InvalidOperationException("The external authorization data cannot be used for authentication.");
        }
        
        var identity = new ClaimsIdentity(
            authenticationType: "ExternalLogin",
            nameType: ClaimTypes.Name,
            roleType: ClaimTypes.Role);
        
        identity
            .SetClaim(ClaimTypes.Email, result.Principal.GetClaim(ClaimTypes.Email))
            .SetClaim(ClaimTypes.Name, result.Principal.GetClaim(ClaimTypes.Name))
            .SetClaim(ClaimTypes.NameIdentifier, result.Principal.GetClaim(ClaimTypes.NameIdentifier));
        
        identity
            .SetClaim(OpenIddictConstants.Claims.Private.RegistrationId, result.Principal.GetClaim(OpenIddictConstants.Claims.Private.RegistrationId))
            .SetClaim(OpenIddictConstants.Claims.Private.ProviderName, result.Principal.GetClaim(OpenIddictConstants.Claims.Private.ProviderName));
        
        var properties = new AuthenticationProperties(result.Properties.Items)
        {
            RedirectUri = result.Properties.RedirectUri ?? "/"
        };
        
        properties.StoreTokens(result.Properties.GetTokens().Where(token => token.Name is
            // Preserve the access and refresh tokens returned in the token response, if available.
            OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken or
            OpenIddictClientAspNetCoreConstants.Tokens.RefreshToken));
        
        return Results.SignIn(new ClaimsPrincipal(identity), properties);


    }
    
}