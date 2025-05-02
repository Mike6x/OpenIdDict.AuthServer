using System.Security.Claims;
using Identity.Domain.Entities;
using Identity.Domain.Settings;
using Shared.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Identity.Infrastructure.Data.Worker;

/// <summary>
/// OpenIdDictWorker is reponsible for setting up the initial values in database
/// </summary>
public class OpenIdDictWorker(
    IServiceProvider serviceProvider, 
    IConfiguration configuration,
    IOptions<CorsOptions> corsOptions
    ) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<IdentityContext>();
        _ = await context.Database.EnsureCreatedAsync(cancellationToken);

        await SeedScopesAsync(scope, cancellationToken);
        await SeedClientsAsync(scope, cancellationToken);
        await SeedRolesAsync(scope);
        await SeedUsersAsync(scope);
                
        // await SeedApplicationsAsync(scope, cancellationToken)
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    
    private static async Task SeedScopesAsync(IServiceScope scope, CancellationToken cancellationToken)
    {
        var scopesManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();
        
        var seedingList = new ScopeCollection().GetAllScopes().ToList();
        
        if (await scopesManager.CountAsync(cancellationToken) == 0 && seedingList.Count != 0)
        {
            foreach (var item in seedingList)
            {
                await scopesManager.CreateAsync(item, cancellationToken);
            }
        }
    }

    private async Task SeedClientsAsync(IServiceScope scope, CancellationToken cancellationToken)
    {
        var applicationManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        var seedingList = new ApplicationCollection(configuration["IdentityHost"]).GetAllApplications().ToList();
        
        var loadingList = configuration.GetSection("OpenIdDict:ApplicationConfigs")
                                .Get<IEnumerable<ApplicationConfig>>();
        
        foreach (var applicationConfig in loadingList ?? [])
        {
            var app = new OpenIddictApplicationDescriptor
            {
                ClientId = applicationConfig.ClientId,
                DisplayName = applicationConfig.DisplayName,
                ClientType = string.IsNullOrWhiteSpace(applicationConfig.ClientSecret)
                    ? ClientTypes.Public
                    : ClientTypes.Confidential,
                ClientSecret = string.IsNullOrWhiteSpace(applicationConfig.ClientSecret)
                    ? null
                    : applicationConfig.ClientSecret,
                Permissions =
                {
                    Permissions.GrantTypes.AuthorizationCode,
                    Permissions.GrantTypes.RefreshToken,
                    Permissions.GrantTypes.Password,
                    
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Introspection,
                    Permissions.Endpoints.Token,
                    Permissions.Endpoints.EndSession,

                    Permissions.ResponseTypes.Code,

                    Permissions.Scopes.Email,
                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Roles, "offline_access",

                    Permissions.Prefixes.Scope + applicationConfig.Scope,
                    Permissions.Prefixes.Scope + Scopes.OfflineAccess,
                },
            };
            if (applicationConfig.PKCE)
            {
                app.Requirements.Add(Requirements.Features.ProofKeyForCodeExchange);
            }

            if (applicationConfig.RedirectUri != null)
                foreach (var uri in applicationConfig.RedirectUri)
                {
                    app.RedirectUris.Add(new Uri(uri));
                }

            if (applicationConfig.PostLogoutRedirectUri != null)
                foreach (var uri in applicationConfig.PostLogoutRedirectUri)
                {
                    app.PostLogoutRedirectUris.Add(new Uri(uri));
                }
            
            seedingList.Add(app);
        }
        
        foreach (var application in seedingList)
        {
            var existApplication = await applicationManager.FindByClientIdAsync(application.ClientId ?? string.Empty, cancellationToken);

            if (existApplication == null) await applicationManager.CreateAsync(application, cancellationToken);

        }
        
        //For each application, add redirect uri to allowed origin list on default cors policy
        var defaultCorsPolicy = corsOptions.Value.GetPolicy(corsOptions.Value.DefaultPolicyName);
        Func<IQueryable<object>, IQueryable<OpenIddictEntityFrameworkCoreApplication>> query = (apps) =>
        {
            return apps.Where(app => true).Select(s => s as OpenIddictEntityFrameworkCoreApplication)!;
        };
        await foreach (var app in applicationManager.ListAsync(query, CancellationToken.None))
        {
            var redirectUris = await applicationManager.GetRedirectUrisAsync(app);
            foreach (var uri in redirectUris.Select(s => new Uri(s)))
            {
                string origin = $"{uri.Scheme}://{uri.Authority}";
                if (!defaultCorsPolicy!.Origins.Contains(origin))
                {
                    defaultCorsPolicy.Origins.Add(origin);
                }
            }
        }

    }
    
    
    private static async Task SeedRolesAsync(IServiceScope scope)
    {
                
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
        
        if (roleManager.Roles.Any()) return;
        
        foreach (var roleName in AppRoles.DefaultRoles)
        {
            var role = new AppRole(roleName, $"{roleName} Role for Identity Server");
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(role);
            }

            switch (roleName)
            {
                case AppRoles.Basic:
                    await AssignPermissionsToRoleAsync(roleManager, role, AppPermissions.Basic);
                    break;
                case AppRoles.Admin:
                    await AssignAdminDefaultPermissionsToRoleAsync(roleManager, role);
                    
                    await AssignPermissionsToRoleAsync(roleManager, role, AppPermissions.Admin);

                    break;
            }
        }
    }

    /// <summary>
    ///  Add Claim from Pixel Admin
    /// </summary>
    /// <param name="roleManager"></param>
    /// <param name="role"></param>
    private static async Task AssignAdminDefaultPermissionsToRoleAsync(
        RoleManager<AppRole> roleManager,
        AppRole role)
    {
        var currentClaims = await roleManager.GetClaimsAsync(role);
        if (!currentClaims.Any())
        {
            await AddClaimAsync(roleManager, role,"identity_read_write", "users");
            await AddClaimAsync(roleManager, role,"identity_read_write", "roles");
            await AddClaimAsync(roleManager, role,"identity_read_write", "applications");
            await AddClaimAsync(roleManager, role,"identity_read_write", "scopes");
        }
    }
    private static async Task AssignPermissionsToRoleAsync(
        RoleManager<AppRole> roleManager,
        AppRole role,
        IReadOnlyList<AppPermission> permissions
        )
    {
        var currentClaims = await roleManager.GetClaimsAsync(role);
        var newClaims = permissions
            .Where(permission => !currentClaims.Any(c => c.Type == AppClaims.Permission && c.Value == permission.Name))
            .ToList();
        
       foreach (var claim in newClaims)
        {
            await AddClaimAsync(roleManager, role,AppClaims.Permission, claim.Name);
        }
    }
    
    private static async Task AddClaimAsync(RoleManager<AppRole> roleManager, AppRole role, string type, string value)
    {
        var claim = new Claim(type, value);
        claim.Properties.Add("IncludeInAccessToken", "true");
        claim.Properties.Add("IncludeInIdentityToken", "true");
        await roleManager.AddClaimAsync(role, claim);
    }
    
    private async Task AssignPermissionsToRoleAsync_fsh(
        IdentityContext dbContext, 
        RoleManager<AppRole> roleManager,
        IReadOnlyList<AppPermission> permissions, 
        AppRole role)
    {
        var currentClaims = await roleManager.GetClaimsAsync(role);
        var newClaims = permissions
            .Where(permission => !currentClaims.Any(c => c.Type == AppClaims.Permission && c.Value == permission.Name))
            .Select(permission => new IdentityRoleClaim
            {
                RoleId = role.Id,
                ClaimType = AppClaims.Permission,
                ClaimValue = permission.Name,
                CreatedAt = DateTime.UtcNow,
              
                //CreatedBy = "application",
                //CreatedOn = timeProvider
                
            })
            .ToList();

        foreach (var claim in newClaims)
        {
            await dbContext.RoleClaims.AddAsync(claim);
        }

        // Save changes to the database context
        if (newClaims.Count != 0)
        {
            await dbContext.SaveChangesAsync();
        }
    }
    
    private async Task SeedUsersAsync(IServiceScope scope)
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        if (userManager.Users.Any()) return;

        var rootConfig =   new UserConfig
            {
                Username = TenantConstants.Root.Name, 
                Email = TenantConstants.Root.EmailAddress , 
                Password = TenantConstants.DefaultPassword,
                Role = AppRoles.Admin
            };
        
        var users = configuration.GetSection("OpenIdDict:Users").Get<IEnumerable<UserConfig>>();

        var seedingList = users != null
            ? users.Append(rootConfig)
            : [rootConfig] ;
        

        foreach (var userConfig in seedingList )
        {
            var user = new AppUser
            {
                UserName = userConfig.Username,
                Email = userConfig.Email,
                EmailConfirmed = true,

                IsActive = true,
                CreatedOn = DateTime.UtcNow
            };
            if (string.IsNullOrWhiteSpace(userConfig.Password))
            {
                userConfig.Password = Guid.NewGuid().ToString();
                //add 3 random upper case letters
                userConfig.Password += new string(Enumerable.Range(0, 3).Select(_ => (char)Random.Shared.Next('A', 'Z')).ToArray());
                Console.WriteLine($"Creating user {userConfig.Email} with password '{userConfig.Password}'");
            }
            var result = await userManager.CreateAsync(user, userConfig.Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, userConfig.Role == "Admin" ? AppRoles.Admin : AppRoles.Basic);
            }
            Console.WriteLine($"Creating user {userConfig.Email}");
        }
    }
    
}