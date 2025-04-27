using System.Text;
using Framework.Core.Exceptions;
using Framework.Core.Mail;
using Framework.Infrastructure.Constants;
using Identity.Domain.Entities;
using Shared.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Services.Users;

public partial class UserService
{
    public async Task SendVerificationEmailAsync(string userId, string origin, CancellationToken cancellationToken)
    {
        var user = await userManager.Users.Where(u => u.Id.ToString() == userId).FirstOrDefaultAsync(cancellationToken)
                   ?? throw new NotFoundException($"User with Id: {userId} doesn't exist.");

        var isAdmin = await userManager.IsInRoleAsync(user, AppRoles.Admin);
        if (isAdmin)
        {
            throw new ConflictException("Administrators do not have been verified");
        }

        user.IsActive = true;
        user.EmailConfirmed = false;

        var result = await userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            var messages = new List<string> { $"User {user.UserName} : " };
            await GenerateVerificationEmail(user, messages, origin,cancellationToken);
        }
    }

    private async Task GenerateVerificationEmail(AppUser user, List<string> messages, string origin, CancellationToken cancellationToken)
    {
        var emailVerificationUri = await GetEmailVerificationUriAsync(user, origin);
        var mailRequest = new MailRequest(
                [user.Email ?? string.Empty],
                "Confirm Registration",
                emailVerificationUri);

        jobService.Enqueue("email", () => mailService.SendAsync(mailRequest, cancellationToken));

        messages.Add($"Please check {user.Email} to verify your account!");
    }
    
    private async Task<string> GetEmailVerificationUriAsync(AppUser user, string origin)
    {
        EnsureValidTenant();

        string code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        //const string route = "api/users/confirm-email"
        const string route = "confirm-email";
        var endpointUri = new Uri(string.Concat($"{origin}/", route));
        string verificationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), QueryStringKeys.UserId, user.Id.ToString());
        verificationUri = QueryHelpers.AddQueryString(verificationUri, QueryStringKeys.Code, code);
        
        // verificationUri = QueryHelpers.AddQueryString(verificationUri,
        //     TenantConstants.Identifier,
        //     multiTenantContextAccessor?.MultiTenantContext?.TenantInfo?.Id!)
        
        return verificationUri;
    }
    
        
    public async Task<string> ConfirmEmailAsync(string userId, string code, string tenant, CancellationToken cancellationToken)
    {
        EnsureValidTenant();

        var user = await userManager.Users.Where(u => u.Id.ToString() == userId ).FirstOrDefaultAsync(cancellationToken) 
                   ?? throw new NotFoundException($"User with Id: {userId} not found!");

        if (user.EmailConfirmed) return  $"Account: {userId}  already confirmed with E-Mail {user.Email} ";

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var result = await userManager.ConfirmEmailAsync(user, code);

        return result.Succeeded
            ? $"Account Confirmed for E-Mail {user.Email}. You can now use the /api/tokens endpoint to generate JWT."
            : $"An error occurred while confirming {user.Email}";
    }
    
    public async Task<string> ConfirmPhoneNumberAsync(string userId, string code, CancellationToken cancellationToken)
    {
        EnsureValidTenant();

        var user = await userManager.FindByIdAsync(userId) ?? throw new NotFoundException($"User with Id: {userId} not found!");

        if (string.IsNullOrEmpty(user.PhoneNumber)) throw new InternalServerException($"User with Id: {userId} have no phone number.");

        var result = await userManager.ChangePhoneNumberAsync(user, user.PhoneNumber, code);
        
        if(!result.Succeeded) throw new InternalServerException($"An error occurred while confirming {user.PhoneNumber}");
        
        return user.PhoneNumberConfirmed
                ? $"Account Confirmed for Phone Number {user.PhoneNumber}. You can now use the /api/tokens endpoint to generate JWT."
                : $"Account Confirmed for Phone Number {user.PhoneNumber}. You should confirm your E-mail before using the /api/tokens endpoint to generate JWT.";

    }

}