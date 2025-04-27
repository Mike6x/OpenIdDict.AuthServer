using System.Security.Claims;
using Identity.Application.Authorization;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using OpenIddict.Client.AspNetCore;
using OpenIddict.Server.AspNetCore;

namespace Identity.Infrastructure.Services.Authorization;

public sealed partial class OpenIdDictService(
    // HttpContext httpContext,
    IOpenIddictApplicationManager applicationManager,
    IOpenIddictAuthorizationManager authorizationManager,
    IOpenIddictScopeManager scopeManager,
    SignInManager<AppUser> signInManager,
    UserManager<AppUser> userManager,
    RoleManager<AppRole> roleManager) : IAuthorizationService
{
    private async Task AddUserClaimsAsync(ClaimsIdentity claimsIdentity, AppUser user)
    {
        foreach(var claim in await userManager.GetClaimsAsync(user))
        {
            claimsIdentity.AddClaim(claim);
        }
        foreach(var assignedRole in await userManager.GetRolesAsync(user))
        {
            var role = await roleManager.FindByNameAsync(assignedRole);
            claimsIdentity.AddClaims(await roleManager.GetClaimsAsync(role));
        }
    }

    private static IEnumerable<string> GetDestinations(Claim claim)
    {
        // Note: by default, claims are NOT automatically included in the access and identity tokens.
        // To allow Authorization to serialize them, you must attach them a destination, that specifies
        // whether they should be included in access tokens, in identity tokens or in both.
       
        switch (claim.Type)
        {
            case OpenIddictConstants.Claims.Name or OpenIddictConstants.Claims.PreferredUsername:
                yield return OpenIddictConstants.Destinations.AccessToken;

                if (claim.Subject != null && claim.Subject.HasScope(OpenIddictConstants.Permissions.Scopes.Profile))
                    yield return OpenIddictConstants.Destinations.IdentityToken;

                yield break;

            case OpenIddictConstants.Claims.Email:
                yield return OpenIddictConstants.Destinations.AccessToken;

                if (claim.Subject != null && claim.Subject.HasScope(OpenIddictConstants.Permissions.Scopes.Email))
                    yield return OpenIddictConstants.Destinations.IdentityToken;

                yield break;

            case OpenIddictConstants.Claims.Role:
                yield return OpenIddictConstants.Destinations.AccessToken;

                if (claim.Subject != null && claim.Subject.HasScope(OpenIddictConstants.Permissions.Scopes.Roles))
                    yield return OpenIddictConstants.Destinations.IdentityToken;

                yield break;

            // Never include the security stamp in the access and identity tokens, as it's a secret value.
            case "AspNet.Identity.SecurityStamp":
                yield break;

            default:
              
                if (claim.Properties.ContainsKey("IncludeInAccessToken"))
                {
                    if (bool.TryParse(claim.Properties["IncludeInAccessToken"], out bool includeInAccessToken)
                        && includeInAccessToken)
                    {
                        yield return OpenIddictConstants.Destinations.AccessToken;
                    }                        
                }
                
                if (claim.Properties.ContainsKey("IncludeInIdentityToken"))
                {                       
                    if (bool.TryParse(claim.Properties["IncludeInIdentityToken"], out bool includeInIdentityToken)
                        && includeInIdentityToken)
                    {
                        yield return OpenIddictConstants.Destinations.IdentityToken;
                    }
                }                  
                yield break;
        }
    }
    
    public async Task<IResult> EndSessionAsync()
    {
        // Ask ASP.NET Core Identity to delete the local and external cookies created
        // when the user agent is redirected from the external identity provider
        // after a successful authentication flow (e.g Google or Facebook).
        await signInManager.SignOutAsync();
        
        // Returning a SignOutResult will ask Authorization to redirect the user agent
        // to the post_logout_redirect_uri specified by the client application or to
        // the RedirectUri specified in the authentication properties if none was set.
        return Results.SignOut(
            authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
            properties: new AuthenticationProperties { RedirectUri = "/" });
    }
    
}