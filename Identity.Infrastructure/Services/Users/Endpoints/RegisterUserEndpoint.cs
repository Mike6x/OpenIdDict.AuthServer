using Identity.Application.Users.Abstractions;
using Identity.Application.Users.Features.RegisterUser;
using Shared.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Users.Endpoints;
public static class SelfRegisterUserEndpoint
{
    internal static RouteHandlerBuilder MapSelfRegisterUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/self-register", (RegisterUserCommand request,
            [FromHeader(Name = TenantConstants.Identifier)] string tenant,
            IUserService service,
            HttpContext context,
            CancellationToken cancellationToken) =>
        {
            var origin = $"{context.Request.Scheme}://{context.Request.Host.Value}{context.Request.PathBase.Value}";
            return service.CreateAsync(request, origin, cancellationToken);
        })
        .WithName(nameof(SelfRegisterUserEndpoint))
        .WithSummary("self register user")
        .WithDescription("self register user")
        .AllowAnonymous();
    }
}
