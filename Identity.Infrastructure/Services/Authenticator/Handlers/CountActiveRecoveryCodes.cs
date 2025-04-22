using Framework.Core.Exceptions;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Services.Authenticator.Endpoints;

public static class CountActiveRecoveryCodes
{
    public static async Task<int> Handler(HttpContext context, UserManager<AppUser> userManager)
    {
        var currentUser = context.User;
        
        var user = await userManager.GetUserAsync(currentUser) 
                   ?? throw new NotFoundException($"Unable to load user with ID '{userManager.GetUserId(currentUser)}'.");
        
        var recoveryCodesCount = await userManager.CountRecoveryCodesAsync(user);
        return recoveryCodesCount;
    }
    
}