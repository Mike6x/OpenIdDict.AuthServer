using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.CorsPolicy.Endpoints;

public static class CorsPolicyEndpoints
{
    public static RouteHandlerBuilder MapAddOrigins(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/add", AddOrigins.Handler)
            .WithName(nameof(MapAddOrigins))
            .WithSummary("Add a list of origin")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Add a list of origins to allowed.");
    }
    
    public static RouteHandlerBuilder MapGetOrigins(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/", GetOrigins.Handler )
            .WithName(nameof(MapGetOrigins))
            .WithSummary("Get a list of allowed origines")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Retrieve a list of all allowed origins.");
    }
    
    public static RouteHandlerBuilder MapRefreshOrigins(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/refresh", RefreshOrigins.Handler)
            .WithName(nameof(MapRefreshOrigins))
            .WithSummary("Get a list of all roles")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Retrieve a list of all roles available in the system.");
    }
    
    public static RouteHandlerBuilder MapRemoveOrigins(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/remove", RemoveOrigins.Handler)
            .WithName(nameof(MapRemoveOrigins))
            .WithSummary("Remove a list of origin")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Remove a list of origins from allowed.");
    }
    
}