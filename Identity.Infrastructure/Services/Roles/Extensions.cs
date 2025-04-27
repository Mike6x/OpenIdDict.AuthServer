using Identity.Infrastructure.Services.Roles.Endpoints.Basic;
using Identity.Infrastructure.Services.Roles.Endpoints.Claim;
using Identity.Infrastructure.Services.Roles.Endpoints.permission;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Roles;

public static class Extensions
{
    public static IEndpointRouteBuilder MapRoleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCreateOrUpdateRoleEndpoint();
        app.MapCreateRoleEndpoint();
        app.MapGetRoleEndpoint();
        app.MapGetRolesEndpoint();
        app.MapSearchRolesEndpoint();
        app.MapUpdateRoleEndpoint();
        app.MapDeleteRoleEndpoint();
        
        return app;
    }
    public static IEndpointRouteBuilder MapRoleClaimEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapAssignClaimsToRoleEndpoint();
        
        app.MapAddClaimToRoleEndpoint();
        app.MapGetRoleClaimsEndpoint();
        app.MapUpdateClaimsToRoleEndpoint();
        app.MapChangeClaimOfRoleEndpoint();
        app.MapRemoveClaimOfRoleEndpoint();

        return app;
    }
    
    public static IEndpointRouteBuilder MapRolePermissionEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetRolePermissionsEndpoint();
        app.MapUpdateRolePermissionsEndpoint();
        
        return app;
    }
}

