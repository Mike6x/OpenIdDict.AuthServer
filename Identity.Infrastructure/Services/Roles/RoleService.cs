using Ardalis.Specification.EntityFrameworkCore;
using Framework.Core.Exceptions;
using Framework.Core.Identity.Users.Abstractions;
using Framework.Core.Paging;
using Framework.Core.Specifications;
using Identity.Application.Claims;
using Identity.Application.Roles;
using Identity.Application.Roles.Features.CreateOrUpdateRole;
using Identity.Application.Roles.Features.SearchRoles;
using Identity.Domain.Entities;
using Identity.Infrastructure.Data;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Authorization;

namespace Identity.Infrastructure.Services.Roles;

public sealed partial class RoleService(RoleManager<AppRole> roleManager,
    UserManager<AppUser> userManager,
    IdentityContext context,
    ICurrentUser currentUser) : IRoleService
{
    public async Task<List<RoleDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await roleManager.Roles.AsNoTracking()
            .Select(role => new RoleDto
                { 
                    Id = role.Id, 
                    Name = role.Name ?? string.Empty, 
                    Description = role.Description })
            .ToListAsync(cancellationToken);
    }
    public async Task<PagedList<RoleDto>> SearchAsync(SearchRolesRequest request, CancellationToken cancellationToken)
    {
        var spec = new EntitiesByPaginationFilterSpec<AppRole>(request);

        var roles = await roleManager.Roles
            .WithSpecification(spec)
            .ProjectToType<RoleDto>()
            .ToListAsync(cancellationToken);

        var count = await roleManager.Roles.CountAsync(cancellationToken);

        return new PagedList<RoleDto>(roles, request.PageNumber, request.PageSize, count);
    }
    public async Task<RoleDto?> GetAsync(string roleId)
    {
        var role = await roleManager.FindByIdAsync(roleId) 
                   ?? throw new NotFoundException($"Role with Id: {roleId} could not be located");
        
        var roleDto = new RoleDto
        {
            Id = role.Id, 
            Name = role.Name?? string.Empty, 
            Description = role.Description
        };
       
        var claims = await roleManager.GetClaimsAsync(role);
        foreach (var claim in claims)
        {
            roleDto.Claims.Add(ClaimViewModel.FromClaim(claim));
            if(claim.Type == AppClaims.Permission) roleDto.Permissions?.Add(claim.Value);
        }
        
        return roleDto;
    }

    public async Task<RoleDto?> GetByNameAsync(string name)
    {
        var role = await roleManager.FindByNameAsync(name) 
                   ?? throw new NotFoundException($"Role with Id: {name} not found");
        var roleDto = new RoleDto
        {
            Id = role.Id, 
            Name = role.Name?? string.Empty, 
            Description = role.Description
        };
       
        var claims = await roleManager.GetClaimsAsync(role);
        foreach (var claim in claims)
        {
            roleDto.Claims.Add(ClaimViewModel.FromClaim(claim));
            if(claim.Type == AppClaims.Permission) roleDto.Permissions?.Add(claim.Value);
        }
        
        return roleDto;
    
    }

    public async Task<RoleDto> CreateOrUpdateAsync(CreateOrUpdateRoleCommand request)
    {
        var role = await roleManager.FindByIdAsync(request.Id);

        if (role != null)
        {
            role.Name = request.Name;
            role.Description = request.Description;
            await roleManager.UpdateAsync(role);
        }
        else
        {
            role = new AppRole(request.Name, request.Description);
            await roleManager.CreateAsync(role);
        }

        return new RoleDto { Id = role.Id, Name = role.Name ?? string.Empty, Description = role.Description };
    }
    public async Task<RoleDto> CreateAsync(CreateRoleCommand request)
    {
        var role = await roleManager.FindByNameAsync(request.Name);

        if (role != null) throw new ConflictException($"Role with Name: {request.Name} already existed.");

        role = new AppRole(request.Name, request.Description);
        
        var result = await roleManager.CreateAsync(role);
        
        if (!result.Succeeded) throw new InternalServerException("CreateRoleAsync failed. ");
        
        role = await roleManager.FindByNameAsync(request.Name) ??  throw new InternalServerException("Internal error. ");
        foreach (var userClaim in request.Claims)
        {
            await roleManager.AddClaimAsync(role, userClaim.ToClaim());
        }
        
        return  new RoleDto
        {
            Id = role.Id, 
            Name = role.Name?? string.Empty, 
            Description = role.Description,
            Claims = request.Claims
        };
    }
    public async Task<RoleDto> UpdateAsync(UpdateRoleCommand request)
    {
        var role = await roleManager.FindByIdAsync(request.Id)
            ?? throw new NotFoundException($"Role with Id: {request.Id} could not be located");
        
        var exists = await roleManager.FindByNameAsync(request.Name);
        
        if(exists != null) throw new ConflictException($"Role: {request.Name} already existed.");
        
        role.Name = request.Name;
        role.Description = request.Description;
        var result = await roleManager.UpdateAsync(role);
       
        if (!result.Succeeded) throw new InternalServerException("CreateRoleAsync failed. ");
        
        return  new RoleDto
        {
            Id = role.Id, 
            Name = role.Name?? string.Empty, 
            Description = role.Description
        };
    }
    
    public async Task DeleteAsync(string roleId)
    {
        var role = await roleManager.FindByIdAsync(roleId) ?? throw new NotFoundException($"Role with Id: {roleId} not found");
        
        var users = await userManager.GetUsersInRoleAsync(role.Name ?? string.Empty);
        
        if (users.Any()) throw new ConflictException($"Role: {role.Name} is in use so can not be deleted.");

        var result =  await roleManager.DeleteAsync(role);
        
        if (!result.Succeeded) throw new InternalServerException("DeleteRoleAsync failed. ");
    }

}
