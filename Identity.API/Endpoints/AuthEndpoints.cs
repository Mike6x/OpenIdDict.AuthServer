using Carter;
using Identity.Infrastructure.Services;
using Identity.Infrastructure.Services.Authentication;
using Identity.Infrastructure.Services.Authenticator;
using Identity.Infrastructure.Services.Authorization;
using Identity.Infrastructure.Services.Clients;
using Identity.Infrastructure.Services.CorsPolicy;
using Identity.Infrastructure.Services.ExternalLogins;
using Identity.Infrastructure.Services.Roles;
using Identity.Infrastructure.Services.Scopes;
using Identity.Infrastructure.Services.Users;

namespace Identity.Api.Endpoints;

public class AuthEndpoints
{
    public class Endpoints : CarterModule
    {
        public Endpoints() : base("identity") { }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            var statusGroup = app.MapGroup("status").WithTags("Status").WithName("Status Group").WithOpenApi();
            statusGroup.MapStatusEndpoints();
            

            var userGroup = app.MapGroup("users").WithTags("Users").WithOpenApi();
            userGroup.MapUserEndpoints();
            
            var userClaimGroup = app.MapGroup("users").WithTags("User Claims").WithOpenApi();
            userClaimGroup.MapUserClaimEndpoints();
            
            var userRoleGroup = app.MapGroup("users").WithTags("User Roles").WithOpenApi();
            userRoleGroup.MapUserRoleEndpoints();
            
            var accountGroup = app.MapGroup("accounts").WithTags("User Accounts").WithOpenApi();
            accountGroup.MapAccountEndpoints();
            
            var currentUserGroup = app.MapGroup("users/Current").WithTags("Current Users").WithOpenApi();
            currentUserGroup.MapCurrentUserEndpoints();
            
            var roleGroup = app.MapGroup("roles").WithTags("Roles").WithOpenApi();
            roleGroup.MapRoleEndpoints();
            
            var roleClaimGroup = app.MapGroup("roles").WithTags("Role Claims").WithOpenApi();
            roleClaimGroup.MapRoleClaimEndpoints();
            
            var rolePermissionGroup = app.MapGroup("roles").WithTags("Roles Permissions").WithOpenApi();
            rolePermissionGroup.MapRolePermissionEndpoints();
            
            
            var clientGroup = app.MapGroup("applications").WithTags("Clients").WithOpenApi();
            clientGroup.MapApplicationEndpoints();
            
            
            var corsPolicyGroup = app.MapGroup("corspolicy") .WithTags("Cors Policy").WithOpenApi();
            corsPolicyGroup.MapCorsPolicyEndpoints();
            
            var scopeGroup = app.MapGroup("scopes").WithTags("Scopes").WithOpenApi();
            scopeGroup.MapScopeEndpoints();
            
            var authenticatorGroup = app.MapGroup("Authenticator").WithTags("Authenticators").WithOpenApi();
            authenticatorGroup.MapAuthenticatorEndpoints();
            
            var externalLoginGroup = app.MapGroup("ExternalLogins").WithTags("External Logins").WithOpenApi();
            externalLoginGroup.MapExternalLoginEndpoints();
            
            var authenticationGroup = app.MapGroup("Auth").WithTags("Authentications").WithOpenApi();
            authenticationGroup.MapAuthenticationEndpoints();
            
            // var openIdConnectGroup = app.MapGroup("")
            // openIdConnectGroup.MapOpenIdDictEndpoints()

        }
    }
    
}