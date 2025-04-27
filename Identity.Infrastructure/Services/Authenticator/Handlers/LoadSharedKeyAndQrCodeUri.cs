using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Services.Authenticator.Handlers;

public static class LoadSharedKeyAndQrCodeUri
{
    public static async Task<(string,string)> Handler(AppUser user, UserManager<AppUser> userManager)
    {
        // Load the authenticator key & QR code URI to display on the form
        var unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(unformattedKey))
        {
            await userManager.ResetAuthenticatorKeyAsync(user);
            unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
        }
          
        var email = await userManager.GetEmailAsync(user);
        var sharedKeyAndQrCode =  (
            AuthenticatorHelper.FormatKey(unformattedKey ?? string.Empty), 
            AuthenticatorHelper.GenerateQrCodeUri(email ?? string.Empty, unformattedKey ?? string.Empty));
        return sharedKeyAndQrCode;
    }

}