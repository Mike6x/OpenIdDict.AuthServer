using System.Security.Claims;
using Framework.Core.Exceptions;
using Framework.Infrastructure.Common.Extensions;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using OpenIddict.Abstractions;
using OpenIddict.Client.AspNetCore;
using OpenIddict.Server.AspNetCore;
using IAuthenticationService = Identity.Application.Authentication.IAuthenticationService;

namespace Identity.Infrastructure.Services.Authentication;

public class AuthenticationService(
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager) : IAuthenticationService
{
    // https://github.com/lieven121/IdentityOidc/blob/main/Identity.App/EndPoints/Identity/IdentityEndpoints.cs
    public async Task<IResult> LogInAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email)) throw new BadRequestException("Email is required");
        
        var user = request.Email.IsValidEmail()
                        ? await userManager.FindByEmailAsync(request.Email.Trim().Normalize())
                        : await userManager.FindByNameAsync(request.Email);
        
        if (user == null) throw new UnauthorizedException("User is not found");
        if (!user.IsActive)
        {
            throw new UnauthorizedException("user is deactivated");
        }

        if (!user.EmailConfirmed)
        {
            throw new UnauthorizedException("user not yet is confirmed by email");
        }
        
        if (user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow)
            throw new UnauthorizedException($"Account is locked until {user.LockoutEnd.Value.ToLocalTime()}");
        
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

        return !result.Succeeded ? Results.Unauthorized() : Results.Ok($"{user.UserName} Logged In");
    }
    
    public async Task<IResult> LogOutAsync(string? returnUrl)
    {
        await signInManager.SignOutAsync();
        
        if(string.IsNullOrWhiteSpace(returnUrl)) returnUrl ="/";
        
        return Results.SignOut(
            authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
            properties: new AuthenticationProperties { RedirectUri = returnUrl });

    }
    
    //https://github.com/RockSolidKnowledge/Samples.OpenIddict.AdminUI/blob/main/Rsk.Samples.OpenIddict.AdminUI/Controllers/AuthenticationController.cs
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
    
