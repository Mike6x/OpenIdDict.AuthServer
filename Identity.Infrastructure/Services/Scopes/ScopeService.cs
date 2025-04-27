using Ardalis.Specification.EntityFrameworkCore;
using Framework.Core.Exceptions;
using Framework.Core.Paging;
using Framework.Core.Specifications;
using Identity.Application.Scopes;
using Identity.Application.Scopes.Features;
using Microsoft.AspNetCore.Http;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;

namespace Identity.Infrastructure.Services.Scopes;

public class ScopeService(
    
    IOpenIddictScopeManager scopeManager, 
    IOpenIddictApplicationManager applicationManager) : IScopeService
{
    public async Task<ScopeViewModel> CreateAsync (ScopeViewModel scopeDescriptor, CancellationToken cancellationToken)
    {
        if (await scopeManager.FindByNameAsync(scopeDescriptor.Name, cancellationToken) is not null)
            throw new ConflictException($"Scope: {scopeDescriptor.Name} have existed");
                
        var openIdScopeDescriptor = scopeDescriptor.ToModel();
                
        var result = await scopeManager.CreateAsync(openIdScopeDescriptor, cancellationToken)
            as OpenIddictEntityFrameworkCoreScope;

        return result == null ? new ScopeViewModel() : result.ToDto();
    }
    
    public async Task<ScopeViewModel> GetAsync(string scopeId, CancellationToken cancellationToken)
    {
        var existing = await scopeManager.FindByIdAsync(scopeId, cancellationToken) 
                           as OpenIddictEntityFrameworkCoreScope
                       ?? throw new NotFoundException($"Scope with id : {scopeId}  doesn't exist");
                
        var descriptor = existing.ToDto();
             
        return descriptor;
    }
    
    public async Task<List<ScopeViewModel>> GetAllAsync (CancellationToken cancellationToken)
    {
        var scopeDescriptors = new List<ScopeViewModel>();

        Func<IQueryable<object>, IQueryable<OpenIddictEntityFrameworkCoreScope>> query 
            = sources => sources.Where(s => true)
                .Select(s => s as OpenIddictEntityFrameworkCoreScope)
                .OrderBy(s => s.Name)!;
            
        await foreach (var scope in scopeManager.ListAsync(query, cancellationToken))
        {
            var descriptor = scope.ToDto();
            scopeDescriptors.Add(descriptor);
        }
            
        return scopeDescriptors;
    }
    
    public  async Task<PagedList<ScopeViewModel>> SearchAsync(SearchScopesRequest request, CancellationToken cancellationToken)
    {
        var spec = new EntitiesByPaginationFilterSpec<OpenIddictEntityFrameworkCoreScope>(request);
            
        List<ScopeViewModel> scopeDescriptors = [];

        Func<IQueryable<object>, IQueryable<OpenIddictEntityFrameworkCoreScope>> query;
            
        query = (apps) => apps.Where(app => true)
            .Select(s => s as OpenIddictEntityFrameworkCoreScope)
            .WithSpecification(spec)
            .OrderBy(s => s.Name);
            
        await foreach (var app in scopeManager.ListAsync(query, cancellationToken))
        {
            var descriptor = app.ToDto();
            scopeDescriptors.Add(descriptor);
        }

        var count = (int)await scopeManager.CountAsync(cancellationToken);
            
        return new PagedList<ScopeViewModel>(scopeDescriptors, request.PageNumber, request.PageSize, count);

    }
    
    public async Task<IResult> DeleteAsync(string scopeId, CancellationToken cancellationToken)
    {
        var existing = await scopeManager.FindByIdAsync(scopeId, cancellationToken) 
                       ?? throw new NotFoundException($"Scope with id : {scopeId}  doesn't exist");
                
        var descriptorFromExisting = new OpenIddictScopeDescriptor();
        await scopeManager.PopulateAsync(descriptorFromExisting, existing, cancellationToken);

        if (string.IsNullOrEmpty(descriptorFromExisting.Name)) throw new ConflictException("Invalid scope name ");

        var count = await applicationManager.CountAsync((Func<IQueryable<object>, IQueryable<OpenIddictEntityFrameworkCoreApplication>>)Query, CancellationToken.None);
        if (count > 0)
        {
            throw new ConflictException( $"Scope is in use by {count} applications so that can not be deleted" );
        }
                                
        await scopeManager.DeleteAsync(existing, cancellationToken);
                
        return Results.Ok();

        IQueryable<OpenIddictEntityFrameworkCoreApplication> Query(IQueryable<object> sources) =>
            sources.Where(s => (s as OpenIddictEntityFrameworkCoreApplication)!.Permissions.Contains(descriptorFromExisting.Name))
                .Select(s => s as OpenIddictEntityFrameworkCoreApplication);
    }
    
    public async Task<IResult> UpdateAsync(ScopeViewModel descriptor, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(descriptor.Id)) return Results.BadRequest();
                
        var existing = await scopeManager.FindByIdAsync(descriptor.Id, cancellationToken) 
                       ?? throw new NotFoundException($"Failed to find Scope with Id: {descriptor.Id}");
                
        var descriptorFromExisting = new OpenIddictScopeDescriptor();
        await scopeManager.PopulateAsync(descriptorFromExisting, existing, cancellationToken);
                
        if (descriptor.Name != descriptorFromExisting.Name 
            && await scopeManager.FindByNameAsync(descriptor.Name, cancellationToken) is not null)
            throw new ConflictException($"Scope: {descriptor.Name} have existed");

        var openIdScopeDescriptor = descriptor.ToModel();
                
        await scopeManager.UpdateAsync(existing, openIdScopeDescriptor, cancellationToken);
                
        return Results.Ok();
    }
}