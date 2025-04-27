using System.Security.Claims;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
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
        await SeedClientsAsync(scope, cancellationToken); // Net 8 Identity
        
        await SeedRolesAsync(scope, cancellationToken, context);
        await SeedUsersAsync(scope);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    
    private async Task SeedScopesAsync(IServiceScope scope, CancellationToken cancellationToken)
    {
        var scopesManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();
        if (await scopesManager.CountAsync(cancellationToken) == 0)
        {
            await scopesManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = "profile",
                DisplayName = "User profile",
                Description = "Access to user profile data",
                Resources = { "identity_server" }
            }, cancellationToken);

            await scopesManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = "email",
                DisplayName = "Email",
                Description = "Access to email address",
                Resources = { "identity_server" }
            }, cancellationToken);

            await scopesManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = "roles",
                DisplayName = "Roles",
                Description = "Access to user roles",
                Resources = { "identity_server" }
            }, cancellationToken);

            await scopesManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = "offline_access",
                DisplayName = "offline_access scope",
            }, cancellationToken);
            
            await scopesManager.CreateAsync(new OpenIddictScopeDescriptor()
            {
                Name = "persistence-api",
                DisplayName = "Persistence Api"
            }, cancellationToken);
            
            await scopesManager.CreateAsync( new OpenIddictScopeDescriptor
            {
                Name = "api",
                Resources = { "resource_server" }
            }, cancellationToken  );
            
            await scopesManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = "api1",
                DisplayName = "Api1 scope",
                Description = "Access to resource server 1",
                Resources = { "resource_server_1" }
            }, cancellationToken);
            
            await scopesManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = AppScopes.CatalogWriteScope,
                Resources =
                {
                    IdentityConstants.CatalogResourceServer,
                    IdentityConstants.GatewayResourceServer
                }
            }, cancellationToken);
            
            await scopesManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = AppScopes.CatalogReadScope,
                Resources =
                {
                    IdentityConstants.CatalogResourceServer,
                    IdentityConstants.GatewayResourceServer
                }
            }, cancellationToken);
            
            await scopesManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = AppScopes.CartWriteScope,
                Resources =
                {
                    IdentityConstants.CartResourceServer,
                    IdentityConstants.GatewayResourceServer
                }
            }, cancellationToken);
            
            await scopesManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = AppScopes.CartReadScope,
                Resources =
                {
                    IdentityConstants.CartResourceServer,
                    IdentityConstants.GatewayResourceServer
                }
            }, cancellationToken);
        }
    }

    private async Task SeedClientsAsync(IServiceScope scope, CancellationToken cancellationToken)
    {
        var applicationManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        if (await applicationManager.FindByClientIdAsync(IdentityConstants.Client, cancellationToken) is null)
        {
            await applicationManager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = IdentityConstants.Client,
                ClientSecret = IdentityConstants.ClientSecret,
                DisplayName = IdentityConstants.ClientDisplayName,
                Permissions =
                {
                    Permissions.Endpoints.Token,
                    Permissions.GrantTypes.ClientCredentials,
                    Permissions.ResponseTypes.Token,
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Roles,
                    Permissions.Prefixes.Scope + AppScopes.CatalogReadScope,
                    Permissions.Prefixes.Scope + AppScopes.CatalogWriteScope,
                    Permissions.Prefixes.Scope + AppScopes.CartReadScope,
                    Permissions.Prefixes.Scope + AppScopes.CartWriteScope
                }
            }, cancellationToken);
        }

        if (await applicationManager.FindByClientIdAsync(IdentityConstants.GatewayResourceServer, cancellationToken) is
            null)
        {
            await applicationManager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = IdentityConstants.GatewayResourceServer,
                ClientSecret = IdentityConstants.GatewayResourceServerSecret,
                Permissions =
                {
                    Permissions.Endpoints.Introspection
                }
            }, cancellationToken);
        }

        if (await applicationManager.FindByClientIdAsync(IdentityConstants.CatalogResourceServer, cancellationToken) is
            null)
        {
            await applicationManager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = IdentityConstants.CatalogResourceServer,
                ClientSecret = IdentityConstants.CatalogResourceServerSecret,
                Permissions =
                {
                    Permissions.Endpoints.Introspection
                }
            }, cancellationToken);
        }

        if (await applicationManager.FindByClientIdAsync("service-worker", cancellationToken) is null)
        {
            await applicationManager.CreateAsync(
                new OpenIddictApplicationDescriptor
                {
                    ClientId = "service-worker",
                    ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C207",
                    Permissions =
                    {
                        Permissions.Endpoints.Authorization,
                        Permissions.Endpoints.Token,
                        Permissions.GrantTypes.ClientCredentials,
                        Permissions.Prefixes.Scope + "cc",
                    },
                },
                cancellationToken
            );
        }

        if (await applicationManager.FindByClientIdAsync("pixel-identity-ui", cancellationToken) is null 
            && !string.IsNullOrEmpty(configuration["IdentityHost"]))
        {

            await applicationManager.CreateAsync(
                new OpenIddictApplicationDescriptor
                {
                    ApplicationType = ApplicationTypes.Web,
                    ClientId = "pixel-identity-ui",
                    ConsentType = ConsentTypes.Implicit,
                    ClientType = ClientTypes.Public,
                    DisplayName = "Pixel Identity",
                    PostLogoutRedirectUris =
                    {
                        new Uri($"{configuration["IdentityHost"]}/authentication/logout-callback")
                    },
                    RedirectUris =
                    {
                        new Uri($"{configuration["IdentityHost"]}/authentication/login-callback")
                    },
                    Permissions =
                    {
                        Permissions.Endpoints.Authorization,
                        Permissions.Endpoints.EndSession,
                        Permissions.Endpoints.Token,
                        Permissions.Endpoints.Introspection,

                        Permissions.GrantTypes.AuthorizationCode,
                        Permissions.GrantTypes.RefreshToken,

                        Permissions.ResponseTypes.Code,
                        Permissions.Scopes.Email,
                        Permissions.Scopes.Profile,
                        Permissions.Scopes.Roles
                    },
                    Requirements =
                    {
                        Requirements.Features.ProofKeyForCodeExchange
                    }
                }, cancellationToken
            );
        }

        if (await applicationManager.FindByClientIdAsync("web-ui", cancellationToken) is null)
        {
            await applicationManager.CreateAsync(
                new OpenIddictApplicationDescriptor
                {
                    ClientId = "web-ui",
                    ConsentType = ConsentTypes.Explicit,
                    DisplayName = "Web UI Client",
                    PostLogoutRedirectUris =
                    {
                        new Uri("https://localhost:4200"),
                        new Uri("https://localhost:3000"),
                        new Uri("https://localhost:7057"),
                    },
                    RedirectUris =
                    {
                        new Uri("https://localhost:4200"),
                        new Uri("https://localhost:3000"),
                        new Uri("https://oauth.pstmn.io/v1/callback"),
                        new Uri("https://localhost:7057/auth/callback"),
                    },
                    Permissions =
                    {
                        Permissions.Endpoints.Authorization,
                        Permissions.Endpoints.Token,
                        Permissions.Endpoints.EndSession,
                        Permissions.Endpoints.Revocation,

                        Permissions.GrantTypes.AuthorizationCode,
                        Permissions.GrantTypes.RefreshToken,

                        Permissions.ResponseTypes.Code,

                        Permissions.Prefixes.Scope + "api",
                        Permissions.Scopes.Email,
                        Permissions.Scopes.Roles,
                        Permissions.Scopes.Profile,
                    },
                    Requirements =
                    {
                        Requirements.Features.ProofKeyForCodeExchange,
                    },
                },
                cancellationToken
            );
        }

        if (await applicationManager.FindByClientIdAsync("mvc-client", cancellationToken) is null)
        {
            await applicationManager.CreateAsync(
                new OpenIddictApplicationDescriptor
                {
                    ClientId = "mvc-client",
                    ClientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0",
                    DisplayName = "MVC Client Application",
                    ConsentType = ConsentTypes.Explicit,

                    RedirectUris = { new Uri("https://localhost:7002/signin-oidc") },
                    PostLogoutRedirectUris =
                    {
                        new Uri("https://localhost:7002/signout-callback-oidc"),
                        new Uri("http://localhost:5002/signout-callback-oidc")
                    },
                    Permissions =
                    {
                        Permissions.Endpoints.Authorization,
                        Permissions.Endpoints.Token,
                        Permissions.Endpoints.Introspection,
                        Permissions.Endpoints.EndSession,
                        Permissions.GrantTypes.AuthorizationCode,
                        Permissions.GrantTypes.RefreshToken,
                        Permissions.GrantTypes.Password,

                        Permissions.Scopes.Email,
                        Permissions.Scopes.Profile,
                        Permissions.Scopes.Roles, "offline_access",
                        Permissions.Prefixes.Scope + "api1",

                        Permissions.ResponseTypes.Code,
                        Permissions.Prefixes.ResponseType + "token",
                        Permissions.Prefixes.ResponseType + "id_token",
                        // OpenIddictConstants.Permissions.Prefixes.ResponseType + "code"
                    },
                },
                cancellationToken);
        }

        if (await applicationManager.FindByClientIdAsync("nextjs-client", cancellationToken) is null)
        {
            await applicationManager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "nextjs-client",
                ConsentType = ConsentTypes.Explicit,
                DisplayName = "Openiddict Plus NextJs UI Client",
                PostLogoutRedirectUris =
                {
                    new Uri("https://localhost:3000"),
                },
                RedirectUris =
                {
                    new Uri("https://localhost:3000/auth/oidc"),
                },
                Permissions =
                {
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Token,
                    Permissions.GrantTypes.AuthorizationCode,
                    Permissions.GrantTypes.RefreshToken,
                    Permissions.Endpoints.EndSession,
                    Permissions.Endpoints.Revocation,
                    Permissions.ResponseTypes.Code,
                    Permissions.Prefixes.Scope + "api",
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Roles,
                    Permissions.Scopes.Profile,
                },
                Requirements =
                {
                    Requirements.Features.ProofKeyForCodeExchange
                }

            }, cancellationToken);
        }
        
        if (await applicationManager.FindByClientIdAsync("blazorwasm-oidc-application", cancellationToken) is null)
        {
            await applicationManager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "blazorwasm-oidc-application",
                ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C206",
                ConsentType = ConsentTypes.Explicit,
                DisplayName = "BlazorWasm Application",
                RedirectUris =
                {
                    new Uri("https://localhost:7002/authentication/login-callback")
                },
                PostLogoutRedirectUris =
                {
                    new Uri("https://localhost:7002/authentication/logout-callback")
                },
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.GrantTypes.Password,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                    $"{OpenIddictConstants.Permissions.Prefixes.Scope}api1"
                },
            }, cancellationToken);
        }


        var applications = configuration.GetSection("OpenIdDict:ApplicationConfigs")
            .Get<IEnumerable<ApplicationConfig>>();
        foreach (var applicationConfig in applications ?? [])
        {
            var client = await applicationManager.FindByClientIdAsync(applicationConfig.ClientId, cancellationToken);

            if (client != null) continue;

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
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Introspection,
                    Permissions.Endpoints.Token,
                    Permissions.Endpoints.EndSession,

                    Permissions.GrantTypes.AuthorizationCode,
                    Permissions.GrantTypes.RefreshToken,
                    Permissions.GrantTypes.Password,

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


            await applicationManager.CreateAsync(app, cancellationToken);

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
    
    private async Task SeedRolesAsync(IServiceScope scope, CancellationToken cancellationToken,  IdentityContext dbContext)
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
    private async Task AssignAdminDefaultPermissionsToRoleAsync(
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
    private async Task AssignPermissionsToRoleAsync(
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