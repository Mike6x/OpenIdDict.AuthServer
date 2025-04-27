using System.Security.Claims;
using Framework.Core.DataIO;
using Framework.Core.Paging;
using Framework.Core.Storage.File.Features;
using Identity.Application.Claims;
using Identity.Application.Claims.DeleteClaim;
using Identity.Application.Claims.Features.Add;
using Identity.Application.Claims.Features.Change;
using Identity.Application.Claims.Features.Update;
using Identity.Application.Users.Dtos;
using Identity.Application.Users.Features.AssignUserRole;
using Identity.Application.Users.Features.ChangePassword;
using Identity.Application.Users.Features.CreateUser;
using Identity.Application.Users.Features.DeleteAccount;
using Identity.Application.Users.Features.ExportUsers;
using Identity.Application.Users.Features.ForgotPassword;
using Identity.Application.Users.Features.ResetPassword;
using Identity.Application.Users.Features.SearchUsers;
using Identity.Application.Users.Features.ToggleUserStatus;
using Identity.Application.Users.Features.UpdateUser;
using Microsoft.AspNetCore.Http;

namespace Identity.Application.Users;

public interface IUserService
{
    # region Basic User CRUD functions

    Task<CreateUserResponse> CreateAsync(CreateUserCommand request, string origin, CancellationToken cancellationToken);
    Task<UserDetail> GetAsync(string userId, CancellationToken cancellationToken);
    Task<UserDto?> GetMeAsync(HttpContext httpContext, CancellationToken cancellationToken);
    Task<List<UserDetail>> GetAllAsync(CancellationToken cancellationToken);
    Task<PagedList<UserDetail>> SearchAsync(SearchUsersRequest request, CancellationToken cancellationToken);
    Task UpdateAsync(UpdateUserCommand request, string userId, CancellationToken cancellationToken);
    Task UpdateProfileAsync(UpdateUserCommand request, string userId, string origin, CancellationToken cancellationToken);
    Task DeleteAsync(string userId);
    
    Task<DashboardStatsDto> GetUserStatisticsAsync(CancellationToken cancellationToken);
    
    #endregion
    
    #region User Import - Export Section
    Task<byte[]> ExportAsync(ExportUsersRequest request, CancellationToken cancellationToken);
    
    Task<ImportResponse> ImportAsync(FileUploadCommand uploadFile, bool isUpdate,string origin, CancellationToken cancellationToken);
    

    #endregion
    
    #region User Managgement Extention

    Task<bool> LockUserAsync(string userId, CancellationToken cancellationToken);
    Task<bool> UnlockUserAsync(string userId, CancellationToken cancellationToken);
    Task<bool> HasPasswordAsync(HttpContext httpContext, CancellationToken cancellationToken);
    
    Task SetActiveStatusAsync(ToggleUserStatusCommand request, CancellationToken cancellationToken);
    Task SetOnlineStatusAsync(string userId, bool isOnline, CancellationToken cancellationToken) ;
    
    #endregion

    #region User Query Section
    
    Task<bool> ExistsWithNameAsync(string name);
    Task<bool> ExistsWithEmailAsync(string email, Guid? exceptId = null);
    Task<bool> ExistsWithPhoneNumberAsync(string phoneNumber, Guid? exceptId = null);
    Task<int> GetCountAsync(CancellationToken cancellationToken);
    
    Task<UserDetail> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task<UserDetail> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<UserDetail> GetByPhoneAsync(string phone, CancellationToken cancellationToken);    

    #endregion
    
    #region User Account Management 
    
    Task ForgotPasswordAsync(ForgotPasswordCommand request, string origin, CancellationToken cancellationToken);
    Task ResetPasswordAsync(ResetPasswordCommand request, CancellationToken cancellationToken);
    Task ChangePasswordAsync(ChangePasswordCommand request, string userId, CancellationToken cancellationToken);

    Task DeleteAccountAsync(HttpContext httpContext, DeleteAccountModel request, CancellationToken cancellationToken);
    
        
    Task<Guid> GetOrCreateFromPrincipalAsync(ClaimsPrincipal principal, CancellationToken cancellationToken);
    
    #endregion
    
    #region User verification section 
    Task SendVerificationEmailAsync(string userId, string origin, CancellationToken cancellationToken);

    Task<string> ConfirmEmailAsync(string userId, string code, string tenant, CancellationToken cancellationToken);
    
    Task<string> ConfirmPhoneNumberAsync(string userId, string code, CancellationToken cancellationToken);

    #endregion
    
    #region User role and permissions Management 

    Task<List<UserRoleDetail>> GetUserRolesAsync(string userId, CancellationToken cancellationToken);
    Task<string> AssignRolesToUserAsync(string userId, AssignUserRoleCommand request, CancellationToken cancellationToken);

    Task<List<string>?> GetUserPermissionsAsync(string userId, CancellationToken cancellationToken);
    Task<List<string>?> GetPermissionsAsync(string userId, CancellationToken cancellationToken);
    
    Task<bool> HasPermissionAsync(string userId, string permission, CancellationToken cancellationToken = default);
    
    #endregion
    
    
    #region User Claim section

    Task<List<ClaimViewModel>> GetUserClaimsAsync(string userId, CancellationToken cancellationToken);
    Task<string> AssignClaimsToUserAsync(string userId, AssignClaimsCommand request, CancellationToken cancellationToken);
    
    Task<bool>AddClaimToUserAsync(string userId, AddClaimCommand request, CancellationToken cancellationToken);
    Task<bool> RemoveClaimOfUserAsync(string userId, RemoveClaimCommand request, CancellationToken cancellationToken);
    Task<bool> ChangeClaimOfUserAsync(string userId, ChangeClaimCommand request, CancellationToken cancellationToken);

    #endregion

}