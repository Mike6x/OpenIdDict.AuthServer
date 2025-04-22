using Identity.Infrastructure.Services.Roles.Endpoints.Basic;
using Identity.Infrastructure.Services.Roles.Endpoints.Claim;
using Identity.Infrastructure.Services.Roles.Endpoints.permission;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Roles.Endpoints;

public static class Extensions
{
    public static IEndpointRouteBuilder MapRoleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetRoleEndpoint();
        app.MapGetRolesEndpoint();
        app.MapSearchRolesEndpoint();
        
        app.MapCreateOrUpdateRoleEndpoint();
        app.MapCreateRoleEndpoint();
        app.MapUpdateRoleEndpoint();
                
        app.MapDeleteRoleEndpoint();
        
        app.MapGetRoleClaimsEndpoint();
        app.MapUpdateClaimsToRoleEndpoint();
        app.MapAssignClaimsToRoleEndpoint();
        app.MapAddClaimToRoleEndpoint();
        app.MapChangeClaimOfRoleEndpoint();
        app.MapRemoveClaimOfRoleEndpoint();

        
        app.MapGetRolePermissionsEndpoint();
        app.MapUpdateRolePermissionsEndpoint();
        
        
        return app;
    }
}

