using Framework.Core.Caching;
using Framework.Core.Exceptions;
using Identity.Application.Users.Dtos;
using Identity.Application.Users.Features.AssignUserRole;
using Shared.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Services.Users;

public sealed partial class UserService
{
    public async Task<List<string>?> GetPermissionsAsync(string userId, CancellationToken cancellationToken)
    {
        var permissions = await cache.GetOrSetAsync(
            GetPermissionCacheKey(userId),
            async () =>
            {
                var user = await userManager.FindByIdAsync(userId) ?? throw new UnauthorizedException();

                var userRoles = await userManager.GetRolesAsync(user);
                var permissions = new List<string>();
                foreach (var role in await roleManager.Roles
                    .Where(r => userRoles.Contains(r.Name!))
                    .ToListAsync(cancellationToken))
                {
                    permissions.AddRange(await dbContext.RoleClaims
                        .Where(rc => rc.RoleId == role.Id && rc.ClaimType == AppClaims.Permission)
                        .Select(rc => rc.ClaimValue!)
                        .ToListAsync(cancellationToken));
                }
                return permissions.Distinct().ToList();
            },
            cancellationToken: cancellationToken);

        return permissions;
    }
    private static string GetPermissionCacheKey(string userId)
    {
        return $"perm:{userId}";
    }

    public async Task<bool> HasPermissionAsync(string userId, string permission, CancellationToken cancellationToken = default)
    {
        var permissions = await GetPermissionsAsync(userId, cancellationToken);

        return permissions?.Contains(permission) ?? false;
    }

    public Task InvalidatePermissionCacheAsync(string userId, CancellationToken cancellationToken)
    {
        return cache.RemoveAsync(GetPermissionCacheKey(userId), cancellationToken);
    }
    
    public async Task<List<UserRoleDetail>> GetUserRolesAsync(string userId, CancellationToken cancellationToken)
    {

        var user = await userManager.FindByIdAsync(userId) 
                   ?? throw new NotFoundException("user not found");
        
        var roles = await roleManager.Roles
                        .AsNoTracking()
                        .OrderBy(r => r.Name)
                        .ToListAsync(cancellationToken) 
                    ?? throw new NotFoundException("roles not found");
        
        var userRoles = new List<UserRoleDetail>();
        foreach (var role in roles)
        {
            userRoles.Add(new UserRoleDetail
            {
                RoleId = role.Id,
                RoleName = role.Name,
                Description = role.Description,
                Enabled = await userManager.IsInRoleAsync(user, role.Name!)
            });
        }

        return userRoles;
    }
    public async Task<string> AssignRolesToUserAsync(string userId, AssignUserRoleCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var user = await userManager.FindByIdAsync(userId)
                   ?? throw new NotFoundException($"User with Id: {userId} doesn't exist.");

        if (await userManager.IsInRoleAsync(user, AppRoles.Admin)
            && request.UserRoles.Exists(a => a is { Enabled: false, RoleName: AppRoles.Admin }))
        {
            // Get count of users in Admin Role
            var adminCount = (await userManager.GetUsersInRoleAsync(AppRoles.Admin)).Count;

            // Check if user is not Root Tenant Admin
            // Edge Case : there are chances for other tenants to have users with the same email as that of Root Tenant Admin. Probably can add a check while User Registration
            
            // if (user.Email == TenantConstants.Root.EmailAddress)
            // {
            //     if (multiTenantContextAccessor?.MultiTenantContext?.TenantInfo?.Id == TenantConstants.Root.Id)
            //     {
            //         throw new GeneralException("action not permitted");
            //     }
            // }
            // else 
            
            if (adminCount <= 2)
            {
                throw new GeneralException("tenant should have at least 2 admins.");
            }
        }

        foreach (var userRole in request.UserRoles)
        {
            if (userRole.RoleName == null || await roleManager.FindByNameAsync(userRole.RoleName) is null) continue;
            
            switch (userRole.Enabled)
            {
                case true when !await userManager.IsInRoleAsync(user, userRole.RoleName):
                    await userManager.AddToRoleAsync(user, userRole.RoleName);
                    break;
                
                case false when await userManager.IsInRoleAsync(user, userRole.RoleName):
                    await userManager.RemoveFromRoleAsync(user, userRole.RoleName);
                    break;
            }
        }

        return "User Handlers Updated Successfully.";

    }

    public async Task<List<string>?> GetUserPermissionsAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId) ?? throw new NotFoundException("user not found");

        var userRoles = await userManager.GetRolesAsync(user);
        var permissions = new List<string>();
        foreach (var role in await roleManager.Roles
                     .Where(r => userRoles.Contains(r.Name!))
                     .ToListAsync(cancellationToken))
        {
            permissions.AddRange(await dbContext.RoleClaims
                .Where(rc => rc.RoleId == role.Id && rc.ClaimType == AppClaims.Permission)
                .Select(rc => rc.ClaimValue!)
                .ToListAsync(cancellationToken));
        }
        return permissions.Distinct().ToList();
    }
}
