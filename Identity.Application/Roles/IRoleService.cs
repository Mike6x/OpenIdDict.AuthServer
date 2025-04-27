using Framework.Core.Paging;
using Identity.Application.Claims;
using Identity.Application.Claims.DeleteClaim;
using Identity.Application.Claims.Features.Add;
using Identity.Application.Claims.Features.Change;

using Identity.Application.Claims.Features.Update;
using Identity.Application.Roles.Features.CreateOrUpdateRole;
using Identity.Application.Roles.Features.SearchRoles;
using Identity.Application.Roles.Features.UpdatePermissions;

namespace Identity.Application.Roles;

public interface IRoleService
{
    Task<List<RoleDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<RoleDto?> GetAsync(string roleId);
    Task<RoleDto?> GetByNameAsync(string name);
    Task<PagedList<RoleDto>> SearchAsync(SearchRolesRequest request, CancellationToken cancellationToken);
    
    Task<RoleDto> CreateAsync(CreateRoleCommand request);
    Task<RoleDto> UpdateAsync(UpdateRoleCommand request);
    Task<RoleDto> CreateOrUpdateAsync(CreateOrUpdateRoleCommand request);

    Task DeleteAsync(string roleId);

    #region Claim Role
    
    Task<List<ClaimViewModel>> GetRoleClaimsAsync(string roleId, CancellationToken cancellationToken);
    Task<bool> AddClaimToRoleAsync(string roleId, AddClaimCommand request, CancellationToken cancellationToken);
    Task<bool> RemoveClaimOfRoleAsync(string roleId, RemoveClaimCommand request, CancellationToken cancellationToken);
    Task<bool> ChangeClaimOfRoleAsync(string roleId,ChangeClaimCommand request, CancellationToken cancellationToken);
    Task<string> UpdateClaimsToRoleAsync(string roleId, AssignClaimsCommand request,CancellationToken cancellationToken);

    Task<string> AssignClaimsToRoleAsync(string roleId, AssignClaimsCommand request,
        CancellationToken cancellationToken);
    #endregion
    
    #region Permission Role

    Task<List<string>> GetRolePermissionsAsync(string roleId, CancellationToken cancellationToken);
    Task<string> UpdatePermissionsToRoleAsync(UpdatePermissionsCommand request);

    #endregion

}
