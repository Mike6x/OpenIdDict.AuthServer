using System.Security.Claims;
using Framework.Infrastructure.Common.Extensions;
using Identity.Domain.Entities;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace Identity.Infrastructure.Services.Authorization.Endpoints;

public static class Authorize
{
    public static async Task<IResult> Handler(HttpContext httpContext,
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        IOpenIddictScopeManager scopeManager
    )
    {
        var request = httpContext.GetOpenIddictServerRequest() 
                      ?? throw new InvalidOperationException( "The OpenID Connect request cannot be retrieved.");
        
        var user = httpContext.User;
        
        if (user.Identity?.IsAuthenticated != true)
            return Results.Challenge();
        
        var claims = new List<Claim>
        {
            new Claim(OpenIddictConstants.Claims.Subject, user.Identity.Name)
        };

        var identity = new ClaimsIdentity(claims, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        var loggedInUser = await userManager.GetUserAsync(user)
                      ?? throw new InvalidOperationException("The user details cannot be retrieved.");


        var principal = await signInManager.CreateUserPrincipalAsync(loggedInUser);

        var emailClaim = principal.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Email));
        if (emailClaim != null)
        {
            var existing = principal.Claims.FirstOrDefault(c => c.Type == OpenIddictConstants.Claims.Email);
            if (existing != null)
                principal.SetClaim(OpenIddictConstants.Claims.Email, emailClaim.Value);
            else 
                principal.AddClaim(OpenIddictConstants.Claims.Email, emailClaim.Value);
        }

        var scopes = request.GetScopes();
        principal.SetScopes(scopes);
        principal.SetResources(await scopeManager.ListResourcesAsync(scopes).ToListAsync());

        foreach (var claim in principal.Claims)
        {
            claim.SetDestinations(GetDestinations(claim, principal));
        }
        

        return Results.SignIn(principal, authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
    
    
    private static IEnumerable<string> GetDestinations(Claim claim, ClaimsPrincipal principal)
    {
        // Note: by default, claims are NOT automatically included in the access and identity tokens.
        // To allow Authorization to serialize them, you must attach them a destination, that specifies
        // whether they should be included in access tokens, in identity tokens or in both.

        switch (claim.Type)
        {
            case OpenIddictConstants.Claims.Subject:
                yield return OpenIddictConstants.Destinations.AccessToken;
                yield return OpenIddictConstants.Destinations.IdentityToken;
                yield break;
            
            case OpenIddictConstants.Claims.Name:
                yield return OpenIddictConstants.Destinations.AccessToken;
                // TODO check
                //if (principal.HasScope(OpenIddictConstants.Permissions.Scopes.Profile))
                yield return OpenIddictConstants.Destinations.IdentityToken;

                yield break;
            case OpenIddictConstants.Claims.GivenName:
                yield return OpenIddictConstants.Destinations.AccessToken;
                if (principal.HasScope(OpenIddictConstants.Permissions.Scopes.Profile))
                    yield return OpenIddictConstants.Destinations.IdentityToken;
                yield break;
            case OpenIddictConstants.Claims.FamilyName:
                yield return OpenIddictConstants.Destinations.AccessToken;
                if (principal.HasScope(OpenIddictConstants.Permissions.Scopes.Profile))
                    yield return OpenIddictConstants.Destinations.IdentityToken;
                yield break;
            case OpenIddictConstants.Claims.Email:
                yield return OpenIddictConstants.Destinations.AccessToken;
                // TODO check
                //if (principal.HasScope(OpenIddictConstants.Permissions.Scopes.Email))
                yield return OpenIddictConstants.Destinations.IdentityToken;

                yield break;

            case OpenIddictConstants.Claims.Role:
                yield return OpenIddictConstants.Destinations.AccessToken;

                if (principal.HasScope(OpenIddictConstants.Permissions.Scopes.Roles))
                    yield return OpenIddictConstants.Destinations.IdentityToken;

                yield break;

            // Never include the security stamp in the access and identity tokens, as it's a secret value.
            case "AspNet.Identity.SecurityStamp": yield break;

            default:
                yield return OpenIddictConstants.Destinations.AccessToken;
                yield break;
        }
    }




}