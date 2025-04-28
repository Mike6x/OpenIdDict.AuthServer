using OpenIddict.Abstractions;

namespace Identity.Infrastructure.Data.Worker;


public class ApplicationCollection
{
    private readonly List<OpenIddictApplicationDescriptor> _applications = [];

    private string IdentityHost { get; set; }

    public IEnumerable<OpenIddictApplicationDescriptor> GetAllApplications() => _applications;

    public ApplicationCollection(string? identityHost)
    {
        IdentityHost = identityHost ?? string.Empty;
        
        // post man client 
        _applications.Add( new OpenIddictApplicationDescriptor
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
        
        // pixel-identity-ui
        _applications.Add(new OpenIddictApplicationDescriptor
        {
            ApplicationType = OpenIddictConstants.ApplicationTypes.Web,
            ClientId = "pixel-identity-ui",
            ConsentType = OpenIddictConstants.ConsentTypes.Implicit,
            ClientType = OpenIddictConstants.ClientTypes.Public,
            DisplayName = "Pixel Identity",
            PostLogoutRedirectUris =
            {
                new Uri($"{IdentityHost}/authentication/logout-callback")
                // new Uri($"{configuration["IdentityHost"]}/authentication/logout-callback")
            },
            RedirectUris =
            {
                new Uri($"{IdentityHost}/authentication/login-callback")
                // new Uri($"{configuration["IdentityHost"]}/authentication/login-callback")
            },
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.EndSession,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.Endpoints.Introspection,

                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Permissions.GrantTypes.RefreshToken,

                OpenIddictConstants.Permissions.ResponseTypes.Code,
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Roles
            },
            Requirements =
            {
                OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange
            }
        });
        
        // blazorwasm-oidc-application
        _applications.Add( new OpenIddictApplicationDescriptor
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
        
        //swagger client 
        _applications.Add( new OpenIddictApplicationDescriptor
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
        
        // console client
        _applications.Add( new OpenIddictApplicationDescriptor
        {
            ClientId = IdentityConstants.Client,
            ClientSecret = IdentityConstants.ClientSecret,
            DisplayName = IdentityConstants.ClientDisplayName,
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                OpenIddictConstants.Permissions.ResponseTypes.Token,
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Roles,
                OpenIddictConstants.Permissions.Prefixes.Scope + AppScopes.CatalogReadScope,
                OpenIddictConstants.Permissions.Prefixes.Scope + AppScopes.CatalogWriteScope,
                OpenIddictConstants.Permissions.Prefixes.Scope + AppScopes.CartReadScope,
                OpenIddictConstants.Permissions.Prefixes.Scope + AppScopes.CartWriteScope
            }
            
        });
        
        // gateway resource server
        _applications.Add( new OpenIddictApplicationDescriptor
        {
            ClientId = IdentityConstants.GatewayResourceServer,
            ClientSecret = IdentityConstants.GatewayResourceServerSecret,
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Introspection
            }
            
        });
        
        // CatalogResourceServer
        _applications.Add(new OpenIddictApplicationDescriptor
        {
            ClientId = IdentityConstants.CatalogResourceServer,
            ClientSecret = IdentityConstants.CatalogResourceServerSecret,
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Introspection
            }
        });
        
        // service-worker
        _applications.Add(new OpenIddictApplicationDescriptor
        {
            ClientId = "service-worker",
            ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C207",
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                OpenIddictConstants.Permissions.Prefixes.Scope + "cc",
            },
        });
        
        // web-ui
        _applications.Add(new OpenIddictApplicationDescriptor
        {
            ClientId = "web-ui",
            ConsentType = OpenIddictConstants.ConsentTypes.Explicit,
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
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.Endpoints.EndSession,
                OpenIddictConstants.Permissions.Endpoints.Revocation,

                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Permissions.GrantTypes.RefreshToken,

                OpenIddictConstants.Permissions.ResponseTypes.Code,

                OpenIddictConstants.Permissions.Prefixes.Scope + "api",
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Roles,
                OpenIddictConstants.Permissions.Scopes.Profile,
            },
            Requirements =
            {
                OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange,
            },
        });
        
        // mvc-client
        _applications.Add(new OpenIddictApplicationDescriptor
        {
            ClientId = "mvc-client",
            ClientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0",
            DisplayName = "MVC Client Application",
            ConsentType = OpenIddictConstants.ConsentTypes.Explicit,

            RedirectUris = { new Uri("https://localhost:7002/signin-oidc") },
            PostLogoutRedirectUris =
            {
                new Uri("https://localhost:7002/signout-callback-oidc"),
                new Uri("http://localhost:5002/signout-callback-oidc")
            },
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.Endpoints.Introspection,
                OpenIddictConstants.Permissions.Endpoints.EndSession,
                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                OpenIddictConstants.Permissions.GrantTypes.Password,

                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Roles, "offline_access",
                OpenIddictConstants.Permissions.Prefixes.Scope + "api1",

                OpenIddictConstants.Permissions.ResponseTypes.Code,
                OpenIddictConstants.Permissions.Prefixes.ResponseType + "token",
                OpenIddictConstants.Permissions.Prefixes.ResponseType + "id_token",
                // OpenIddictConstants.Permissions.Prefixes.ResponseType + "code"
            },
        });
        
        //nextjs-client
        _applications.Add(new OpenIddictApplicationDescriptor
        {
            ClientId = "nextjs-client",
            ConsentType = OpenIddictConstants.ConsentTypes.Explicit,
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
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                OpenIddictConstants.Permissions.Endpoints.EndSession,
                OpenIddictConstants.Permissions.Endpoints.Revocation,
                OpenIddictConstants.Permissions.ResponseTypes.Code,
                OpenIddictConstants.Permissions.Prefixes.Scope + "api",
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Roles,
                OpenIddictConstants.Permissions.Scopes.Profile,
            },
            Requirements =
            {
                OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange
            }

        });
        
        // react-client
        _applications.Add( new OpenIddictApplicationDescriptor
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

        // client-credentials-oidc-application  
        _applications.Add( new OpenIddictApplicationDescriptor
        {
            ClientId = "client-credentials-oidc-application",
            ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C201",
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.GrantTypes.ClientCredentials
            }
        });
        
        // password-oidc-application
        _applications.Add( new OpenIddictApplicationDescriptor
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
}
