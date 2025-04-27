using Identity.Application.Roles;
using Identity.Application.Roles.Features.CreateOrUpdateRole;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Roles.Endpoints.Basic;

public static class CreateOrUpdateRoleEndpoint
{
    public static RouteHandlerBuilder MapCreateOrUpdateRoleEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/CreateAndUpdate", async (CreateOrUpdateRoleCommand request, IRoleService service) =>
        {
            return await service.CreateOrUpdateAsync(request);
        })
        .WithName(nameof(CreateOrUpdateRoleEndpoint))
        .WithSummary("Create or update a role")
        // .RequirePermission("Permissions.Handlers.Create")
        .WithDescription("Create a new role or update an existing role.");
    }
}

public static class CreateRoleEndpoint
{
    public static RouteHandlerBuilder MapCreateRoleEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/", async (CreateRoleCommand request, IRoleService roleService) =>
            {
                return await roleService.CreateAsync(request);
            })
            .WithName(nameof(CreateRoleEndpoint))
            .WithSummary("Create a role")
            // .RequirePermission("Permissions.Handlers.Create")
            .WithDescription("Create a new role .");
    }
}

public static class UpdateRoleEndpoint
{
    public static RouteHandlerBuilder MapUpdateRoleEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPut("/", async (UpdateRoleCommand request, IRoleService roleService) =>
            {
                return await roleService.UpdateAsync(request);
            })
            .WithName(nameof(UpdateRoleEndpoint))
            .WithSummary("Update a role")
            // .RequirePermission("Permissions.Handlers.Create")
            .WithDescription("Update an existing role.");
    }
}

