using BuildingBlocks.Exceptions;
using Identity.Application.Users.Abstractions;
using Identity.Application.Users.Dtos;
using Identity.Application.Users.Features.RegisterUser;
using Identity.Domain.Enums;
using Identity.Domain.Models;
using Mapster;
using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Services.Users;

public sealed partial class UserService(
    UserManager<AppUser> userManager
    // SignInManager<AppUser> signInManager,
    // RoleManager<AppRole> roleManager,
    // IdentityDbContext db
) : IUserService
{
    private void EnsureValidTenant()
    {
        // if (string.IsNullOrWhiteSpace(multiTenantContextAccessor?.MultiTenantContext?.TenantInfo?.Id))
        // {
        //     throw new UnauthorizedException("invalid tenant");
        // }
    }
    public async Task<bool> ExistsWithEmailAsync(string email, string? exceptId = null)
    {
        EnsureValidTenant();
        return await userManager.FindByEmailAsync(email.Normalize()) is { } user && user.Id != exceptId;
    }

    public async Task<bool> ExistsWithNameAsync(string name)
    {
        EnsureValidTenant();
        return await userManager.FindByNameAsync(name) is not null;
    }

    public async Task<bool> ExistsWithPhoneNumberAsync(string phoneNumber, string? exceptId = null)
    {
        EnsureValidTenant();
        return await userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber) is { } user && user.Id != exceptId;
    }

    public async Task<UserDetail> GetAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync(cancellationToken);

        _ = user ?? throw new NotFoundException($"User with Id: {userId} not found!");

        return user.Adapt<UserDetail>();
    }

    public Task<int> GetCountAsync(CancellationToken cancellationToken)
    {
        return userManager.Users.AsNoTracking().CountAsync(cancellationToken);
    }

    public async Task<List<UserDetail>> GetListAsync(CancellationToken cancellationToken)
    {
        var users = await userManager.Users.AsNoTracking().ToListAsync(cancellationToken);
        return users.Adapt<List<UserDetail>>();
    }

    public async Task<RegisterUserResponse> RegisterAsync(RegisterUserCommand request, string origin, CancellationToken cancellationToken)
    {
        // create user entity
        var user = new AppUser
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.UserName ?? request.Email,
            PhoneNumber = request.PhoneNumber,
            Status = UserStatus.Active,
            EmailConfirmed = false,
        };

        // register user
        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(error => error.Description).ToList();
            throw new  BadRequestException ("error while registering a new user");
            // throw new FshException("error while registering a new user", errors);
        }

        // add basic role
        // await userManager.AddToRoleAsync(user, FshRoles.Basic);
        //
        // // send confirmation mail
        // if (!string.IsNullOrEmpty(user.Email))
        // {
        //     string emailVerificationUri = await GetEmailVerificationUriAsync(user, origin);
        //     var mailRequest = new MailRequest(
        //         new Collection<string> { user.Email },
        //         "Confirm Registration",
        //         emailVerificationUri);
        //     jobService.Enqueue("email", () => mailService.SendAsync(mailRequest, CancellationToken.None));
        // }

        return new RegisterUserResponse(user.Id);
    }

}