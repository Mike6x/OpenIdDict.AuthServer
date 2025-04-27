using Framework.Core.Exceptions;
using Identity.Application.Claims;
using Identity.Application.Claims.DeleteClaim;
using Identity.Application.Claims.Features.Add;
using Identity.Application.Claims.Features.Change;
using Identity.Application.Claims.Features.Update;
using Identity.Application.Roles;
using Mapster;
using Shared.Authorization;

namespace Identity.Infrastructure.Services.Roles;

public partial class RoleService
{
    public async Task<List<ClaimViewModel>> GetRoleClaimsAsync(string roleId, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(roleId) 
                   ?? throw new NotFoundException($"Role with Id: {roleId} could not be located");

        var roleClaims = await roleManager.GetClaimsAsync(role);
        
        var claims = new List<ClaimViewModel>();
        claims.AddRange(roleClaims.Select(ClaimViewModel.FromClaim));
        
        return claims;
    }
    
    public async Task<bool> AddClaimToRoleAsync(string roleId, AddClaimCommand request, CancellationToken cancellationToken )
    {
        var role = await roleManager.FindByNameAsync(request.Owner)
                   ?? throw new NotFoundException($"Role : {request.Owner} not found.");
        
        var currentClaims = await roleManager.GetClaimsAsync(role);
        
        if (currentClaims.Any(a => a.Type.Equals(request.ClaimToAdd.Type) 
                                   && a.Value.Equals(request.ClaimToAdd.Value)))
        {
            throw new ConflictException($"Role: {request.Owner} already have this claim.");
        }
        
        var result =  await roleManager.AddClaimAsync(role, request.ClaimToAdd.ToClaim());
        return result.Succeeded;
    }
    
    public async Task<bool> ChangeClaimOfRoleAsync( string roleId, ChangeClaimCommand request, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByNameAsync(request.Owner)
                   ?? throw new NotFoundException($"Role : {request.Owner} not found.");
        
        var currentClaims = await roleManager.GetClaimsAsync(role);
        var claimToRemove = currentClaims.FirstOrDefault(c => c.Type.Equals(request.Original.Type)
                                                              && c.Value.Equals(request.Original.Value));
        if (claimToRemove == null)  throw new NotFoundException($"Role: {request.Owner} do not have remove claim.{request.Original.Value}");
        
        var removeResult = await roleManager.RemoveClaimAsync(role, claimToRemove);
        
        var claimToAdd = currentClaims.FirstOrDefault(c => c.Type.Equals(request.Modified.Type)
                                                           && c.Value.Equals(request.Modified.Value));
        if (claimToAdd != null) return removeResult.Succeeded;
        
        var addResult = await roleManager.AddClaimAsync(role, request.Modified.ToClaim());
        return removeResult.Succeeded && addResult.Succeeded;
    }
    
    public async Task<bool> RemoveClaimOfRoleAsync( string roleId, RemoveClaimCommand request,CancellationToken cancellationToken )
    {
        var role = await roleManager.FindByNameAsync(request.Owner)
                   ?? throw new NotFoundException($"Role : {request.Owner} not found.");
        
        var currentClaims = await roleManager.GetClaimsAsync(role);
        
        var claimToRemove = currentClaims.FirstOrDefault(a => a.Type.Equals(request.ClaimToRemove.Type) 
                                                              && a.Value.Equals(request.ClaimToRemove.Value))
                            ?? throw new ConflictException($"Role: {request.Owner} do not have request claim {request.ClaimToRemove.Value}.");
        
        var result = await roleManager.RemoveClaimAsync(role, claimToRemove);
        return result.Succeeded;
    }

    // clone from UpdatePermissionsToRoleAsync
    public async Task<string> UpdateClaimsToRoleAsync(string roleId, AssignClaimsCommand request, CancellationToken cancellationToken )
    {
        var role = await roleManager.FindByIdAsync(request.Owner)
                   ?? throw new NotFoundException($"Role with Id: {request.Owner} not found");
        
        var currentClaims = await roleManager.GetClaimsAsync(role);

        // Remove current claims not in request list
        foreach (var claim in currentClaims.Where(c => !request.Claims.Exists(
                     r => r.Type.Equals(c.Type) && r.Value.Equals(c.Value))))
        {
            var result = await roleManager.RemoveClaimAsync(role, claim);
            if (result.Succeeded) continue;
            var errors = result.Errors.Select(error => error.Description).ToList();
            throw new GeneralException("operation failed", errors);
        }

        // Add all request claims except which have existed
        foreach (var claim  in request.Claims.Where(r => currentClaims.All(
                     c => c.Type != r.Type && c.Value != r.Value)))
        {
            await roleManager.AddClaimAsync(role, claim.ToClaim());
        }
        
        return "Claims updated successfully";
    }
    
    // clone assign role to user
    public async Task<string> AssignClaimsToRoleAsync(string roleId, AssignClaimsCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var role = await roleManager.FindByIdAsync(request.Owner)
                   ?? throw new NotFoundException($"Role with Id: {request.Owner} not found");
        
        var currentClaims = await roleManager.GetClaimsAsync(role);

        foreach (var claim in request.Claims)
        {
            if (claim.Enabled)
            {
                if (!currentClaims.Any(a => a.Type.Equals(claim.Type) && a.Value.Equals(claim.Value)))
                {
                    await roleManager.AddClaimAsync(role, claim.ToClaim());
                }
            }
            else
            {
                if (currentClaims.Any(a => a.Type.Equals(claim.Type) && a.Value.Equals(claim.Value)))
                {
                    await roleManager.RemoveClaimAsync(role, claim.ToClaim());
                }

            }
        }

        return "User Handlers Updated Successfully.";

    }
}