using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;

namespace Identity.Infrastructure.Data.Worker;

public class ClientSeeder(IServiceProvider serviceProvider)
{
    public async Task AddScopes()
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

        var apiScope = await manager.FindByNameAsync("api1");
        if (apiScope != null) await manager.DeleteAsync(apiScope);

        await manager.CreateAsync(new OpenIddictScopeDescriptor
        {
            DisplayName = "Api scope",
            Name = "api1",
            Resources =
            {
                "resource_server_1"
            }
        });
    }
    
    public async Task AddClientCredentialsClient()
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<IdentityContext>();
        await context.Database.EnsureCreatedAsync();
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        var client = await manager.FindByClientIdAsync("client-credentials-oidc-application");
        if (client != null) await manager.DeleteAsync(client);

        await manager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = "client-credentials-oidc-application",
            ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C201",
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.GrantTypes.ClientCredentials
            }
        });
    }
    
    public async Task AddPasswordClient()
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<IdentityContext>();
        await context.Database.EnsureCreatedAsync();
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        var client = await manager.FindByClientIdAsync("password-oidc-application");
        if (client != null) await manager.DeleteAsync(client);

        await manager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = "password-oidc-application",
            ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C202",
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.GrantTypes.Password
            }
        });
    }
    
    public async Task AddAuthorizationClient()
        {
            await using var scope = serviceProvider.CreateAsyncScope();

            var context = scope.ServiceProvider.GetRequiredService<IdentityContext>();
            await context.Database.EnsureCreatedAsync();

            var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            var client = await manager.FindByClientIdAsync("authorization-oidc-application");
            if (client != null)
            {
                await manager.DeleteAsync(client);
            }

            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "authorization-oidc-application",
                ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C203",
                ConsentType = OpenIddictConstants.ConsentTypes.Explicit,
                DisplayName = "Postman client application",
                RedirectUris =
                {
                    new Uri("https://oidcdebugger.com/debug")
                },
                PostLogoutRedirectUris =
                {
                    new Uri("https://oauth.pstmn.io/v1/callback")
                },
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Authorization,
                    OpenIddictConstants.Permissions.Endpoints.EndSession,
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.Permissions.ResponseTypes.Code,
                    OpenIddictConstants.Permissions.Scopes.Email,
                    OpenIddictConstants.Permissions.Scopes.Profile,
                    OpenIddictConstants.Permissions.Scopes.Roles,
                   $"{OpenIddictConstants.Permissions.Prefixes.Scope}api1"
                },
                //Requirements =
                //{
                //    Requirements.Features.ProofKeyForCodeExchange
                //}
            });
        }
    
    public async Task AddBlazorWasmClient()
    {
        await using var scope = serviceProvider.CreateAsyncScope();

        var context = scope.ServiceProvider.GetRequiredService<IdentityContext>();
        await context.Database.EnsureCreatedAsync();

        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        var client = await manager.FindByClientIdAsync("blazorwasm-oidc-application");
        if (client != null)
        {
            await manager.DeleteAsync(client);
        }

        await manager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = "blazorwasm-oidc-application",
            ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C206",
            ConsentType = OpenIddictConstants.ConsentTypes.Explicit,
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
        });
    }
    

    public async Task AddReactClient()
    {
        await using var scope = serviceProvider.CreateAsyncScope();

        var context = scope.ServiceProvider.GetRequiredService<IdentityContext>();
        await context.Database.EnsureCreatedAsync();

        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        
        var reactClient = await manager.FindByClientIdAsync("react-client");
        if (reactClient != null)
        {
            await manager.DeleteAsync(reactClient);
        }

        await manager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = "react-client",
            ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C2014",
            ConsentType = OpenIddictConstants.ConsentTypes.Explicit,
            DisplayName = "React client application",
            RedirectUris =
            {
                new Uri("http://localhost:3000/oauth/callback")
            },
            PostLogoutRedirectUris =
            {
                new Uri("http://localhost:3000/")
            },
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.EndSession,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Permissions.ResponseTypes.Code,
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Roles,
                $"{OpenIddictConstants.Permissions.Prefixes.Scope}api1"
            },
            //Requirements =
            //{
            //    Requirements.Features.ProofKeyForCodeExchange
            //}
        });
    }
        
    public async Task AddWebClient()
    {
        await using var scope = serviceProvider.CreateAsyncScope();

        var context = scope.ServiceProvider.GetRequiredService<IdentityContext>();
        await context.Database.EnsureCreatedAsync();

        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        var client = await manager.FindByClientIdAsync("web-client");
        if (client != null)
        {
            await manager.DeleteAsync(client);
        }

        await manager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = "web-client",
            ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C205",
            ConsentType = OpenIddictConstants.ConsentTypes.Explicit,
            DisplayName = "Swagger client application",
            RedirectUris =
            {
                new Uri("https://localhost:7002/swagger/oauth2-redirect.html")
            },
            PostLogoutRedirectUris =
            {
                new Uri("https://localhost:7002/resources")
            },
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.EndSession,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Permissions.ResponseTypes.Code,
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Roles,
               $"{OpenIddictConstants.Permissions.Prefixes.Scope}api1"
            },
            //Requirements =
            //{
            //    Requirements.Features.ProofKeyForCodeExchange
            //}
        });
    }

    
    
}