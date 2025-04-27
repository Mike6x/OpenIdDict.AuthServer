using Framework.Core.Exceptions;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Services.Authenticator.Handlers;

public static class GenerateRecoverCodes
{
    public static async Task<IEnumerable<string>?> Handler(HttpContext context, UserManager<AppUser> userManager)
    {
        var currentUser = context.User;
        
        var user = await userManager.GetUserAsync(currentUser) 
                   ?? throw new NotFoundException($"Unable to load user with ID '{userManager.GetUserId(currentUser)}'.");

        var isTwoFactorEnabled = await userManager.GetTwoFactorEnabledAsync(user);
        if (!isTwoFactorEnabled)
            throw new BadRequestException("2FA is not enabled for account." );                
        

        var recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
        return recoveryCodes;
    }

}
