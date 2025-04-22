using Framework.Core.Exceptions;
using Identity.Application.Clients;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;

namespace Identity.Infrastructure.Services.Clients.Endpoints;

public static class  ApplicationEndpoints
{
    public static RouteHandlerBuilder MapCreateApplicationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/", CreateApplication.Handler)
            .WithName(nameof(CreateApplication))
            .WithSummary("Create new application")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Create new Application.");
    }
    
    public static RouteHandlerBuilder MapGetApplicationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{clientId}",GetApplication.Handler)
            .WithName(nameof(GetApplication))
            .WithSummary("Get application details ")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Retrieve the details of a role by its ID.");
    }
    
    public static RouteHandlerBuilder MapGetApplicationsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/", GetApplications.Handler)
            .WithName(nameof(GetApplications))
            .WithSummary("Get all clients ")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Return a list of all Applications.");
    }
    
    public static RouteHandlerBuilder MapSearchApplicationsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/search", SearchApplications.Handler)
            .WithName(nameof(SearchApplications))
            .WithSummary("Search applications ")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Return a Paged list of Applications.");
    }
    
    public static RouteHandlerBuilder MapDeleteApplicationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapDelete("/{clientId}", DeleteApplication.Handler)
            .WithName(nameof(DeleteApplication))
            .WithSummary("Remove application.")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Remove Application.");
    }
    
    public static RouteHandlerBuilder MapUpdateApplicationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPut("/", UpdateApplication.Handler)
            .WithName(nameof(UpdateApplication))
            .WithSummary("Update application.")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Update Application.");
    }
    
    public static RouteHandlerBuilder MapCallBackApplicationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/callback", CallBackApplication.Handler)
            .WithName(nameof(MapCallBackApplicationEndpoint))
            .WithSummary("Call back application.")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Call back Application.");
    }
}
