using Framework.Core.Exceptions;
using Identity.Domain.Entities;
using Identity.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Services.Authenticator.Endpoints;

public static class SetUser2FaStatus
{
    /// <summary>
    /// 1. Enable: is2FaEnnable = true, isReset = false
    /// 2. Disable: is2FaEnnable = false, isReset = fail
    /// 3, Reset: is2FaEnnable = false, isReset = true
    /// </summary>
    /// <param name="context"></param>
    /// <param name="code"></param>
    /// <param name="is2FaEnabled"></param>
    /// <param name="isReset"></param>
    /// <param name="userManager"></param>
    /// <param name="signInManager"></param>
    /// <returns></returns>
    /// <exception cref="BadRequestException"></exception>
    /// <exception cref="NotFoundException"></exception>
    public static async Task<IResult> EnableHandler(HttpContext context, string code,  
        UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        return await Handler(context, code, true, false, userManager, signInManager);
    }

    public static async Task<IResult> DisableHandler(HttpContext context, string code,  
        UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        return await Handler(context, code, false, false, userManager, signInManager);
    }
    public static async Task<IResult> ResetHandler(HttpContext context, string code,  
        UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        return await Handler(context, code, false, true, userManager, signInManager);
    }
    private static async Task<IResult> Handler(HttpContext context, string code, 
        bool is2FaEnabled,  bool isReset, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        if (string.IsNullOrEmpty(code)) 
            throw new BadRequestException("Verification code can not be null or empty." );
        
        var currentUser = context.User;
        
        var user = await userManager.GetUserAsync(currentUser) 
                   ?? throw new NotFoundException($"Unable to load user with ID '{userManager.GetUserId(currentUser)}'.");
        
        // Strip spaces and hyphens
        var verificationCode = code.Replace(" ", string.Empty).Replace("-", string.Empty);

        var is2FaTokenValid = await userManager.VerifyTwoFactorTokenAsync(user,
            userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

        if (!is2FaTokenValid)  
            throw new BadRequestException("Verification code is invalid." );
        
        var settingResult = await userManager.SetTwoFactorEnabledAsync(user, is2FaEnabled);
        if (!settingResult.Succeeded)
        {
            throw new BadRequestException(string.Join(Environment.NewLine, settingResult.GetErrors()));
        }

        if (isReset)
        {
            await userManager.ResetAuthenticatorKeyAsync(user);
            await userManager.GetUserIdAsync(user);         
            await signInManager.RefreshSignInAsync(user);
        }
        
        return Results.Ok(); 
    }
    
}
