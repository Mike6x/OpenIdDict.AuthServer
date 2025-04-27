using Framework.Core.Exceptions;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Services.Authenticator.Handlers;

public static class IsAuthenticatorEnabled 
{
    public static async Task<bool> Handler(HttpContext context, UserManager<AppUser> userManager)
    {
        var currentUser = context.User;
        
        var user = await userManager.GetUserAsync(currentUser) 
                   ?? throw new NotFoundException($"Unable to load user with ID '{userManager.GetUserId(currentUser)}'.");

        var isTwoFactorEnabled = await userManager.GetTwoFactorEnabledAsync(user);
        
        return isTwoFactorEnabled;
    }

}
