using Identity.Application.Users.Abstractions;
using Identity.Application.Users.Features.CreateUser;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Users.Endpoints;

public static class CreateUserEndpoint
{
    internal static RouteHandlerBuilder MapCreateUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/", (CreateUserCommand request,
                IUserService service,
                HttpContext context,
                CancellationToken cancellationToken) =>
            {
                var origin = $"{context.Request.Scheme}://{context.Request.Host.Value}{context.Request.PathBase.Value}";
                return service.CreateAsync(request, origin, cancellationToken);
            })
            .WithName(nameof(CreateUserEndpoint))
            .WithSummary("Create user")
            // .RequirePermission("Permissions.Users.Create")
            //.MapToApiVersion(1)
            .WithDescription("Create new user");
    }
}