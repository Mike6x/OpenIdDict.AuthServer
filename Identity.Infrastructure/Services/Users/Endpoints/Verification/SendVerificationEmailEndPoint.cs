using Identity.Application.Users.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Users.Endpoints
{
    public static class SendVerificationEmailEndPoint
    {
        internal static RouteHandlerBuilder MapSendVerificationEmailEndPoint(this IEndpointRouteBuilder endpoints)
        {
            return endpoints.MapPost("/{id:guid}/verification-email", async (
                string id,
                HttpContext context,
                [FromServices] IUserService userService,
                CancellationToken cancellationToken) =>
                {
                    // using with get endpoint
                    // var origin = $"{context.Request.Scheme}://{context.Request.Host.Value}{context.Request.PathBase.Value}"

                    var originUrl = context.Request.Headers.Origin;

                    await userService.SendVerificationEmailAsync(id, originUrl!, cancellationToken);
                    return Results.Ok();
                })
                .WithName(nameof(SendVerificationEmailEndPoint))
                .WithSummary("Send email to verify user")
                // .RequirePermission("Permissions.Endpoints.Update")
                .WithDescription("Send email to verify user");
        }
    }
}
