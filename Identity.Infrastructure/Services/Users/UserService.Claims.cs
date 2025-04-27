using System.Security.Claims;
using Framework.Core.Exceptions;
using Identity.Application.Claims;
using Identity.Application.Claims.DeleteClaim;
using Identity.Application.Claims.Features.Add;
using Identity.Application.Claims.Features.Change;
using Identity.Application.Claims.Features.Update;

namespace Identity.Infrastructure.Services.Users;

public partial class UserService
{
    public async Task<List<ClaimViewModel>> GetUserClaimsAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId)
                   ?? throw new NotFoundException($"User with Id: {userId} doesn't exist.");

        var userClaims = await userManager.GetClaimsAsync(user)
            ?? throw new NotFoundException($"User with Id: {userId} does not have any claims.");
        
        var claims = new List<ClaimViewModel>();
        claims.AddRange(userClaims.Select(ClaimViewModel.FromClaim));
        
        return claims;
    }
    public async Task<bool> AddClaimToUserAsync(string userId, AddClaimCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Owner)
                   ?? throw new NotFoundException($"User with Id: {request.Owner} doesn't exist.");

        var currentClaims = await userManager.GetClaimsAsync(user);
        
        if (currentClaims.Any(a => a.Type.Equals(request.ClaimToAdd.Type) && a.Value.Equals(request.ClaimToAdd.Value)))
        {
            throw new ConflictException("User with Id: " + request.Owner + "already have assigned this claim.");
        }
        
        var result =  await userManager.AddClaimAsync(user, request.ClaimToAdd.ToClaim());
        
        return result.Succeeded;
    }
    public async Task<bool> ChangeClaimOfUserAsync(string userId, ChangeClaimCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Owner)
                   ?? throw new NotFoundException($"User with Id: {request.Owner} doesn't exist.");
        

        var currentClaims = await userManager.GetClaimsAsync(user);
        var claimToRemove = currentClaims.FirstOrDefault(c => c.Type.Equals(request.Original.Type)
                                                              && c.Value.Equals(request.Original.Value));
        if (claimToRemove == null) throw new NotFoundException($"Remove Claim not found for {request.Original.Value}.");
        
        var removeResult = await userManager.RemoveClaimAsync(user, claimToRemove);
        
        var claimToAdd = currentClaims.FirstOrDefault(c => c.Type.Equals(request.Modified.Type)
                                                              && c.Value.Equals(request.Modified.Value));
        if (claimToAdd != null) return removeResult.Succeeded;
        
        var addResult =  await userManager.AddClaimAsync(user, request.Modified.ToClaim());
               
        return removeResult.Succeeded && addResult.Succeeded;
    }
    public async Task<bool> RemoveClaimOfUserAsync(string userId, RemoveClaimCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Owner)
                   ?? throw new NotFoundException($"User with Id: {request.Owner} doesn't exist.");
 
        var currentClaims = await userManager.GetClaimsAsync(user);

        var claimToRemove = currentClaims.FirstOrDefault(a => a.Type.Equals(request.ClaimToRemove.Type) && a.Value.Equals(request.ClaimToRemove.Value));

        if (claimToRemove == null) 
            throw new NotFoundException($"User with Id: {request.Owner} doesn't have request claim {request.ClaimToRemove.Value}.");
        
        var result = await userManager.RemoveClaimAsync(user, claimToRemove);
                
        return result.Succeeded;

    }
    
    // clone assign role to user
    public async Task<string> AssignClaimsToUserAsync(string userId, AssignClaimsCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var user = await userManager.FindByIdAsync(userId)
                   ?? throw new NotFoundException($"User with Id: {userId} doesn't exist.");
        
        var currentClaims = await userManager.GetClaimsAsync(user);

        foreach (var clame in request.Claims)
        {
            if (clame.Enabled)
            {
                if (!currentClaims.Any(a => a.Type.Equals(clame.Type) && a.Value.Equals(clame.Value)))
                {
                    await userManager.AddClaimAsync(user, clame.ToClaim());
                }
            }
            else
            {
                if (currentClaims.Any(a => a.Type.Equals(clame.Type) && a.Value.Equals(clame.Value)))
                {
                    await userManager.RemoveClaimAsync(user, clame.ToClaim());
                }

            }
        }

        return "User Handlers Updated Successfully.";

    }
}