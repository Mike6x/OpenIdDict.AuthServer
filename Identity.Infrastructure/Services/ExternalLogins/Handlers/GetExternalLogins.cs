using System.Security.Claims;
using Framework.Core.Exceptions;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Services.ExternalLogins.Handlers;

public static class GetExternalLogins
{
    public static async Task<List<UserLoginInfo>> Handler(ClaimsPrincipal userPrincipal,UserManager<AppUser> userManager)
        {
            var user = await userManager.GetUserAsync(userPrincipal)
                       ?? throw new NotFoundException($"User could not be loaded.");

            var currentLogins = await userManager.GetLoginsAsync(user);
            
            return currentLogins.ToList();
        }
}