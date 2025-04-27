using Ardalis.Specification.EntityFrameworkCore;
using Framework.Core.Caching;
using Framework.Core.DataIO;
using Framework.Core.Exceptions;
using Framework.Core.Jobs;
using Framework.Core.Mail;
using Framework.Core.Paging;
using Framework.Core.Specifications;
using Framework.Core.Storage;
using Framework.Core.Storage.File;
using Identity.Application.Users;
using Identity.Application.Users.Dtos;
using Identity.Application.Users.Features.CreateUser;
using Identity.Application.Users.Features.SearchUsers;
using Identity.Application.Users.Features.UpdateUser;
using Identity.Domain.Entities;
using Identity.Infrastructure.Data;
using Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Services.Users;

public sealed partial class UserService(
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    RoleManager<AppRole> roleManager,
    IdentityContext dbContext,
    ICacheService cache,
    IJobService jobService,
    IMailService mailService,
    IStorageService storageService,
    IDataExport dataExport,
    IDataImport dataImport
) : IUserService
{
    /// <summary>
    /// Basic User CRUD functions
    /// </summary>
    private void EnsureValidTenant()
    {
        // if (string.IsNullOrWhiteSpace(multiTenantContextAccessor?.MultiTenantContext?.TenantInfo?.Id))
        // {
        //     throw new UnauthorizedException("invalid tenant");
        // }
    }
    
    public async Task<CreateUserResponse> CreateAsync(CreateUserCommand request, string origin, CancellationToken cancellationToken)
    {
        var user = new AppUser
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.UserName ?? request.Email,
            PhoneNumber = request.PhoneNumber,
            IsActive = true,
            EmailConfirmed = false,
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(error => error.Description).ToList();
            throw new GeneralException("error while registering a new user", errors);
        }

        await userManager.AddToRoleAsync(user, AppRoles.Basic);
        
        // send confirmation mail
        if (!string.IsNullOrEmpty(user.Email))
        {
            string emailVerificationUri = await GetEmailVerificationUriAsync(user, origin);
            var mailRequest = new MailRequest(
                [user.Email],
                "Confirm Registration",
                emailVerificationUri);
            jobService.Enqueue("email", () => mailService.SendAsync(mailRequest, CancellationToken.None));
        }

        return new CreateUserResponse(user.Id);
    }

    public async Task<List<UserDetail>> GetAllAsync(CancellationToken cancellationToken)
    {
        var users = await userManager.Users.AsNoTracking().ToListAsync(cancellationToken);
        return users.Adapt<List<UserDetail>>();
    }

    public async Task<UserDetail> GetAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .AsNoTracking()
            .Where(u => u.Id.ToString() == userId)
            .FirstOrDefaultAsync(cancellationToken);

        _ = user ?? throw new NotFoundException($"User with Id: {userId} not found!");

        return user.Adapt<UserDetail>();
    }
    
    public async Task<UserDto?> GetMeAsync(HttpContext httpContext, CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(httpContext.User);
        
        return user == null 
            ? null 
            : new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    IsActive = user.IsActive,
                    EmailConfirmed = user.EmailConfirmed,
                };
    }

    public async Task<PagedList<UserDetail>> SearchAsync(SearchUsersRequest request, CancellationToken cancellationToken)
    {
        var spec = new EntitiesByPaginationFilterSpec<AppUser>(request);

        var users = await userManager.Users
            .WithSpecification(spec)
            .ProjectToType<UserDetail>()
            .ToListAsync(cancellationToken);

        var count = await userManager.Users
            .CountAsync(cancellationToken);

        return new PagedList<UserDetail>(users, request.PageNumber, request.PageSize, count);
    }

    public async Task DeleteAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId)
                   ?? throw new NotFoundException($"User with Id: {userId} Not Found.");

        if (user.Email == TenantConstants.Root.EmailAddress)
        {
            throw new ConflictException($"Admin user: {user.Email} can not be deleted !");
        }

        var userRoles = await userManager.GetRolesAsync(user);
        if (userRoles.Contains(AppRoles.Admin))
        {
            throw new ConflictException($"User with role: {AppRoles.Admin} can not be deleted !");
        }
        
        if (user.ImageUrl != null)
        {
            storageService.Remove(user.ImageUrl);
            user.ImageUrl = null;
        }

        var result = await userManager.RemoveFromRolesAsync(user, userRoles);

        if (!result.Succeeded)
        {
            throw new GeneralException("Remove role(s) failed.");
        }

        await signInManager.RefreshSignInAsync(user);
        result = await userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(error => error.Description).ToList();
            throw new GeneralException("Remove profile failed", errors);
        }
    }

    public async Task UpdateAsync(UpdateUserCommand request, string userId, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId) 
                   ?? throw new NotFoundException("$user with Id: {userId} not found");

        var imageUri = user.ImageUrl ?? null;
        
        if (request.Image != null || request.DeleteCurrentImage)
        {
            user.ImageUrl = await storageService.UploadAsync<AppUser>(request.Image, FileType.Image);
            if (request.DeleteCurrentImage && imageUri != null)
            {
                storageService.Remove(imageUri);
            }
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.PhoneNumber;
        var phoneNumber = await userManager.GetPhoneNumberAsync(user);
        if (request.PhoneNumber != phoneNumber)
        {
            await userManager.SetPhoneNumberAsync(user, request.PhoneNumber);
        }

        user.IsOnline = request.IsOnline ?? user.IsOnline;

        var result = await userManager.UpdateAsync(user);
        await signInManager.RefreshSignInAsync(user);

        if (!result.Succeeded)
        {
            throw new GeneralException("Update profile failed");
        }
    }

    public async Task UpdateProfileAsync(UpdateUserCommand request, string userId, string origin, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId) 
            ?? throw new NotFoundException("$user with Id: {userId} not found");
        var imageUri = user.ImageUrl ?? null;
        if (request.Image != null || request.DeleteCurrentImage)
        {
            user.ImageUrl = await storageService.UploadAsync<AppUser>(request.Image, FileType.Image);
            if (request.DeleteCurrentImage && imageUri != null)
            {
                storageService.Remove(imageUri);
            }
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.UserName = request.UserName ?? request.Email;
        user.PhoneNumber = request.PhoneNumber;

        user.IsOnline = request.IsOnline ?? user.IsOnline;
        user.EmailConfirmed = request is { IsActive: true, EmailConfirmed: true };
        user.IsActive = request.IsActive;
        user.LockoutEnd = request.LockoutEnd;

        var result = await userManager.UpdateAsync(user);
        
        // send verification email
        var messages = new List<string> { $"User {user.UserName} Updated." };
               
        if (result.Succeeded && request is { IsActive: true, EmailConfirmed: false })
        {
            await GenerateVerificationEmail(user, messages, origin, cancellationToken);
        }

        // Change Password
        if (result.Succeeded && request is { Password: not null, ConfirmPassword: not null } && request.Password == request.ConfirmPassword)
        {
            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
            var resetPassResult = await userManager.ResetPasswordAsync(user, resetToken, request.Password);
            if (!resetPassResult.Succeeded)
            {
                throw new InternalServerException("Change password failed");
            }
        }

        await signInManager.RefreshSignInAsync(user);

        if (!result.Succeeded)
        {
            throw new GeneralException("Update profile failed");
        }
    }


    public async Task<DashboardStatsDto> GetUserStatisticsAsync(CancellationToken cancellationToken)
    {
        var users = await userManager.Users.AsNoTracking().ToListAsync(cancellationToken);
        
        var stats = new DashboardStatsDto
        {
            TotalUsers =  users.Count,
            ActiveUsers = users.Count(u => u.IsActive),
            OnlineUsers = users.Count(u => u.IsOnline == true),
            LockedUsers = users.Count(u=> u.LockoutEnd != null && u.LockoutEnd > DateTime.UtcNow),
            
            NewUsersToday = users.Count(u => u.CreatedOn.Date == DateTime.UtcNow.Date),
            RecentUsers = users.Adapt<List<RecentUserDto>>().OrderBy(u => u.CreatedOn)

        };

        return stats;
    }
}