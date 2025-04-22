using Identity.Application.Scopes;
using Identity.Application.Scopes.Features;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Scopes.Endpoints;

public static class ScopeEndpoints
{
    public static RouteHandlerBuilder MapCreateScopeEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/", CreateScope.Handler)
            .WithName(nameof(MapCreateScopeEndpoint))
            .WithSummary("Create new Scope")
            // .RequirePermission("Permissions.Endpoints.View")
            .WithDescription("Create new Scope.");
    }
    
    public static RouteHandlerBuilder MapGetScopeEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{scopeId}",async (string scopeId, IScopeService service, CancellationToken cancellationToken) 
                => await service.GetAsync(scopeId, cancellationToken))
            .WithName(nameof(MapGetScopeEndpoint))
            .WithSummary("Get scope details ")
            // .RequirePermission("Permissions.Endpoints.View")
            .WithDescription("Retrieve the details of a scope by its ID.");
    }
    
    public static RouteHandlerBuilder MapGetScopesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/", async (IScopeService service, CancellationToken cancellationToken) 
                => await service.GetAllAsync(cancellationToken))
            .WithName(nameof(MapGetScopesEndpoint))
            .WithSummary("Get all Scopes")
            // .RequirePermission("Permissions.Endpoints.View")
            .WithDescription("Retrieve a list of all Scope available in the system.");
    }
    
    public static RouteHandlerBuilder MapSearchScopesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/search",(SearchScopesRequest request, IScopeService service, CancellationToken cancellationToken) =>
            {
                return service.SearchAsync(request, cancellationToken);
            })
            .WithName(nameof(MapSearchScopesEndpoint))
            .WithSummary("Search Scopes ")
            // .RequirePermission("Permissions.Endpoints.View")
            .WithDescription("Return a Paged list of Scopes.");
    }
    
    public static RouteHandlerBuilder MapDeleteScopeEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapDelete("/{scopeId}", DeleteScope.Handler)
            .WithName(nameof(MapDeleteScopeEndpoint))
            .WithSummary("Remove Scope.")
            // .RequirePermission("Permissions.Endpoints.View")
            .WithDescription("Remove a scope from the system by its ID");
    }
    
    public static RouteHandlerBuilder MapUpdateScopeEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPut("/", UpdateScope.Handler)
            .WithName(nameof(MapUpdateScopeEndpoint))
            .WithSummary("Update Scope.")
            // .RequirePermission("Permissions.Endpoints.View")
            .WithDescription("Update Scope.");
    }
}