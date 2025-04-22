using System.Collections.ObjectModel;
using System.Text;
using Framework.Core.Exceptions;
using Framework.Core.Mail;
using Identity.Application.Users.Features.ChangePassword;
using Identity.Application.Users.Features.ForgotPassword;
using Identity.Application.Users.Features.ResetPassword;
using Microsoft.AspNetCore.WebUtilities;
using Shared.Authorization;

namespace Identity.Infrastructure.Services.Users;

public partial class UserService
{
    public async Task ForgotPasswordAsync(ForgotPasswordCommand request, string origin, CancellationToken cancellationToken)
    {
        EnsureValidTenant();
    
        var user = await userManager.FindByEmailAsync(request.Email) ?? throw new NotFoundException("user not found");
        if (!await userManager.IsEmailConfirmedAsync(user))
        {
            throw new InvalidOperationException("user email not yet confirmed");
        }
    
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
    
        string resetPasswordUri = $"{origin}/reset-password?token={token}&email={request.Email}";
    
        var mailRequest = new MailRequest(
            new Collection<string> { user.Email! },
            "Reset Password",
            $"Please reset your password using the following link: {resetPasswordUri}");
    
        jobService.Enqueue(() => mailService.SendAsync(mailRequest, CancellationToken.None));
    }

    public async Task ResetPasswordAsync(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        EnsureValidTenant();

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            throw new NotFoundException("user not found");
        }

        request.Token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
        var result = await userManager.ResetPasswordAsync(user, request.Token, request.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            throw new GeneralException("error resetting password", errors);
        }
    }

    
    public async Task ChangePasswordAsync(ChangePasswordCommand request, string userId)
    {
        var user = await userManager.FindByIdAsync(userId)
                   ?? throw new NotFoundException("user not found");

        var result = await userManager.ChangePasswordAsync(user, request.Password, request.NewPassword);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            throw new GeneralException("failed to change password", errors);
        }
    }
    
    public async Task<bool> HasPassword(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }
        return await userManager.HasPasswordAsync(user);
    }
    
    
    public async Task DeleteAccountAsync(string userId)
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
            throw new GeneralException("Delete profile failed", errors);
        }
    }

}
