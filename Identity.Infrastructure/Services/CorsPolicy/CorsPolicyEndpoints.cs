using Identity.Infrastructure.Services.CorsPolicy.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.CorsPolicy;

public static class AddOriginsEndpoint
{
    public static RouteHandlerBuilder MapAddOriginsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/add", AddOrigins.Handler)
            .WithName(nameof(AddOriginsEndpoint))
            .WithSummary("Add a list of origin")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Add a list of origins to allowed.");
    }
}

public static class GetOriginsEndpoint
{
    public static RouteHandlerBuilder MapGetOriginsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/", GetOrigins.Handler )
            .WithName(nameof(GetOriginsEndpoint))
            .WithSummary("Get a list of allowed origines")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Retrieve a list of all allowed origins.");
    }
}

public static class RefreshOriginsEndpoint
{
    public static RouteHandlerBuilder MapRefreshOriginsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/refresh", RefreshOrigins.Handler)
            .WithName(nameof(RefreshOriginsEndpoint))
            .WithSummary("Get a list of all roles")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Retrieve a list of all roles available in the system.");
    }
}

public static class RemoveOriginsEndpoint
{
    public static RouteHandlerBuilder MapRemoveOriginsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/remove", RemoveOrigins.Handler)
            .WithName(nameof(RemoveOriginsEndpoint))
            .WithSummary("Remove a list of origin")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Remove a list of origins from allowed.");
    }
}