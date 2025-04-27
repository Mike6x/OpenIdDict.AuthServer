using Framework.Core.Exceptions;
using Identity.Application.Users.Features.ToggleUserStatus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Authorization;

namespace Identity.Infrastructure.Services.Users;

public partial class UserService
{
    public async Task <bool> LockUserAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId)
                   ?? throw new NotFoundException($"User with Id: {userId} doesn't exist.");

        var result = await userManager.SetLockoutEnabledAsync(user, true);
        await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.Now.AddDays(90));
        await userManager.UpdateSecurityStampAsync(user);
        
        return result.Succeeded;
    }
    
    public async Task<bool> UnlockUserAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId)
                   ?? throw new NotFoundException($"User with Id: {userId} doesn't exist.");
        

        var result = await userManager.SetLockoutEndDateAsync(user, null);
        
        return result.Succeeded;
  
    }
    
    public async Task SetActiveStatusAsync(ToggleUserStatusCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users.Where(u => u.Id.ToString() == request.UserId).FirstOrDefaultAsync(cancellationToken)
                   ?? throw new NotFoundException($"User With ID: {request.UserId} Not Found.");

        var isAdmin = await userManager.IsInRoleAsync(user, AppRoles.Admin);
        if (isAdmin)
        {
            throw new GeneralException("Administrators Profile's Status cannot be toggled");
        }

        user.IsActive = request.IsActive;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(error => error.Description).ToList();
            throw new GeneralException("Changing user active  status operation failed", errors);
        }
    }
    
    public async Task SetOnlineStatusAsync(string userId, bool isOnline, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId) ?? throw new NotFoundException($"User With ID: {userId} Not Found.");

        user.IsOnline = isOnline;

        await userManager.UpdateAsync(user);
    }

}