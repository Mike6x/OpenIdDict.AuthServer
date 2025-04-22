using System.Security.Claims;
using Framework.Core.Exceptions;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Services.ExternalLogins.Handlers;


public static class DeleteExternalLoginEndpoint
{
    public static async Task<IResult> Handler(
            HttpContext httpContext,
                string loginProvider,
                string providerKey,
                ClaimsPrincipal userPrincipal,
                UserManager<AppUser> userManager,
                SignInManager<AppUser> signInManager,
                IUserStore<AppUser> userStore,
                CancellationToken cancellationToken
            )
            {
                var user = await userManager.GetUserAsync(userPrincipal)
                           ?? throw new NotFoundException($"User could not be loaded.");

                var currentLogins = await userManager.GetLoginsAsync(user);
                
                var passwordHash = string.Empty;

                if (userStore is IUserPasswordStore<AppUser> userPasswordStore)
                {
                    passwordHash = await userPasswordStore.GetPasswordHashAsync(user, httpContext.RequestAborted);
                }

                if (!string.IsNullOrEmpty(passwordHash) || currentLogins.Count > 1)
                {
                    var result = await userManager.RemoveLoginAsync(user, loginProvider, providerKey);
                    if (!result.Succeeded)
                    {
                       return Results.Problem($"Error while removing external login provider: {loginProvider}");
                    }

                    await signInManager.RefreshSignInAsync(user);
                    return Results.Ok();
                }

                return Results.Problem($"Error while removing external login provider: {loginProvider}");
            }
}