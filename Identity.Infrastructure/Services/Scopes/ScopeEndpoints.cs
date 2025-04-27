using Identity.Application.Scopes;
using Identity.Application.Scopes.Features;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Scopes;

public static class CreateScopeEndpoint
{
    public static RouteHandlerBuilder MapCreateScopeEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/", (ScopeViewModel request, IScopeService service, CancellationToken cancellationToken) 
                => service.CreateAsync(request, cancellationToken))
            .WithName(nameof(CreateScopeEndpoint))
            .WithSummary("Create new Scope")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Create new Scope.");
    }
}

public static class GetScopeEndpoint
{
    public static RouteHandlerBuilder MapGetScopeEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{scopeId}",async (string scopeId, IScopeService service, CancellationToken cancellationToken) 
                => await service.GetAsync(scopeId, cancellationToken))
            .WithName(nameof(GetScopeEndpoint))
            .WithSummary("Get scope details ")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Retrieve the details of a scope by its ID.");
    }
}

public static class GetScopesEndpoint
{
    public static RouteHandlerBuilder MapGetScopesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/", async (IScopeService service, CancellationToken cancellationToken) 
                => await service.GetAllAsync(cancellationToken))
            .WithName(nameof(GetScopesEndpoint))
            .WithSummary("Get all Scopes")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Retrieve a list of all Scope available in the system.");
    }
}

public static class SearchScopesEndpoint
{
    public static RouteHandlerBuilder MapSearchScopesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/search",(SearchScopesRequest request, IScopeService service, CancellationToken cancellationToken) 
                => service.SearchAsync(request, cancellationToken))
            .WithName(nameof(SearchScopesEndpoint))
            .WithSummary("Search Scopes ")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Return a Paged list of Scopes.");
    }
}

public static class DeleteScopeEndpoint
{
    public static RouteHandlerBuilder MapDeleteScopeEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapDelete("/{scopeId}", async (string scopeId, IScopeService service, CancellationToken cancellationToken) =>
            {
                await service.DeleteAsync(scopeId, cancellationToken);
            })
            .WithName(nameof(DeleteScopeEndpoint))
            .WithSummary("Remove Scope.")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Remove a scope from the system by its ID");
    }
}


public static class UpdateScopeEndpoint
{
    public static RouteHandlerBuilder MapUpdateScopeEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPut("/", (ScopeViewModel request, IScopeService service, CancellationToken cancellationToken) 
                => service.UpdateAsync(request, cancellationToken))
            .WithName(nameof(UpdateScopeEndpoint))
            .WithSummary("Update Scope.")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Update Scope.");
    }
}