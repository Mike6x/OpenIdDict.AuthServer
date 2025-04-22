using Identity.Infrastructure.Services.Users.Endpoints.Account;
using Identity.Infrastructure.Services.Users.Endpoints.BasicFeatures;
using Identity.Infrastructure.Services.Users.Endpoints.Claim;
using Identity.Infrastructure.Services.Users.Endpoints.CurrentUser;
using Identity.Infrastructure.Services.Users.Endpoints.ManagementExtensions;
using Identity.Infrastructure.Services.Users.Endpoints.Roles;
using Identity.Infrastructure.Services.Users.Endpoints.Verification;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Users.Endpoints;

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
        
        app.MapDisableUserEndpoint();
        app.MapToggleUserStatusEndpoint();
        app.MapUnLockUserEndpoint();
        app.MapLockUserEndpoint();
        
        app.MapGetUserByEmailEndpoint();
        app.MapGetUserByNameEndpoint();
        app.MapGetUserByPhoneNumberEndpoint();
        app.MapGetOtherUsersEndpoint();
        
        app.MapGetCurrentUserEndpoint(); 
        app.MapUpdateCurrentUserEndpoint();
        app.MapGetCurrentUserPermissionsEndpoint();   
        app.MapLogoutCurrentUserEndpoint();
        
        app.MapGetUserRolesEndpoint();
        app.MapAssignRolesToUserEndpoint();
        app.MapGetUserPermissionsEndpoint();

        app.MapGetUserClaimsEndpoint();
        app.MapAddClaimToUserEndpoint();
        app.MapAssignClaimsToUserEndpoint();
        app.MapChangeClaimOfUserEndpoint();
        app.MapRemoveClaimOfUserEndpoint();
        
        
        //app.MapGetUserAuditTrailEndpoint()
        app.MapGetUsersDashboardEndpoint();
        return app;
    }

}