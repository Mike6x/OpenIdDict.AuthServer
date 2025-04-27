using Identity.Infrastructure.Services.Users.Endpoints;
using Identity.Infrastructure.Services.Users.Endpoints.Account;
using Identity.Infrastructure.Services.Users.Endpoints.BasicFeatures;
using Identity.Infrastructure.Services.Users.Endpoints.Claim;
using Identity.Infrastructure.Services.Users.Endpoints.CurrentUser;
using Identity.Infrastructure.Services.Users.Endpoints.ManagementExtensions;
using Identity.Infrastructure.Services.Users.Endpoints.Passwords;
using Identity.Infrastructure.Services.Users.Endpoints.Roles;
using Identity.Infrastructure.Services.Users.Endpoints.Verification;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Users;

public static class Extensions
{
    public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapChangePasswordEndpoint();
        app.MapForgotPasswordEndpoint();
        app.MapResetPasswordEndpoint();
        
        app.MapGetCornfirmEmailEndpoint();
        app.MapGetCornfirmPhoneNumberEndpoint();
        app.MapCornfirmEmailEndpoint();
        app.MapSendVerificationEmailEndPoint();

        app.MapHasPasswordEndpoint();
        app.MapDeleteAccountEndpoint();
        
        return app;
    }
    
    public static IEndpointRouteBuilder MapCurrentUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetCurrentUserEndpoint(); 
        app.MapUpdateCurrentUserEndpoint();
        app.MapGetCurrentUserPermissionsEndpoint();
        app.MapGetMeEndpoint();
        
        return app;
    }
    
    public static IEndpointRouteBuilder MapUserClaimEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapAssignClaimsToUserEndpoint();
        
        app.MapAddClaimToUserEndpoint();
        app.MapGetUserClaimsEndpoint();
        
        app.MapChangeClaimOfUserEndpoint();
        app.MapRemoveClaimOfUserEndpoint();
        
        return app;
    }
    
    public static IEndpointRouteBuilder MapUserRoleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapAssignRolesToUserEndpoint();
        app.MapGetUserRolesEndpoint();
        
        app.MapGetUserPermissionsEndpoint();

        return app;
    }
    
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapRegisterUserEndpoint();
        
        app.MapCreateUserEndpoint();
        app.MapGetUsersEndpoint();
        app.MapGetUserEndpoint();
        app.MapSearchUsersEndpoint();
        app.MapDeleteUserEndpoint();
        app.MapUpdateUserEndpoint();
        
        app.MapExportUsersEndpoint();
        app.MapImportUsersEndpoint();
        
        app.MapLockUserEndpoint();
        app.MapUnLockUserEndpoint();
        app.MapToggleUserStatusEndpoint();
        app.MapToggleOnlineStatusEndpoint();
        
        app.MapGetUserByEmailEndpoint();
        app.MapGetUserByNameEndpoint();
        app.MapGetUserByPhoneNumberEndpoint();
        app.MapGetOtherUsersEndpoint();
        
        app.MapGetUserStatisticsEndpoint();
        //app.MapGetUserAuditTrailEndpoint()
        
        return app;
    }

}