using Framework.Core.Exceptions;
using Identity.Domain.Entities;
using Identity.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Services.Authenticator.Endpoints;

public static class GetAuthenticatorAndUri
{
    public static async Task<IResult>  Handler(HttpContext context, UserManager<AppUser> userManager)
    {
        var currentUser = context.User;
        
        var user = await userManager.GetUserAsync(currentUser) 
                   ?? throw new NotFoundException($"Unable to load user with ID '{userManager.GetUserId(currentUser)}'.");
        
        var sharedKeyAndQrCode = await LoadSharedKeyAndQrCodeUri.Handler(user, userManager);
          
        return Results.Ok(new EnableAuthenticatorModel()
        {
            SharedKey = sharedKeyAndQrCode.Item1,
            AuthenticatorUri = sharedKeyAndQrCode.Item2
        });
    }

}
