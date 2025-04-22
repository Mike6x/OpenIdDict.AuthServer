using FluentValidation;
using FluentValidation.Results;
using Framework.Core.Origin;
using Identity.Application.Users.Abstractions;
using Identity.Application.Users.Features.ChangePassword;
using Shared.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace Identity.Infrastructure.Services.Users.Endpoints;
public static class ChangePasswordEndpoint
{
    internal static RouteHandlerBuilder MapChangePasswordEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/change-password", async (ChangePasswordCommand command,
            HttpContext context,
            IOptions<OriginOptions> settings,
            [FromServices] IValidator<ChangePasswordCommand> validator,
            IUserService userService,
            CancellationToken cancellationToken) =>
        {
            ValidationResult result = await validator.ValidateAsync(command, cancellationToken);
            if (!result.IsValid)
            {
                return Results.ValidationProblem(result.ToDictionary());
            }

            if (context.User.GetUserId() is not { } userId || string.IsNullOrEmpty(userId))
            {
                return Results.BadRequest();
            }

            await userService.ChangePasswordAsync(command, userId);
            return Results.Ok("password have changed");
        })
        .WithName(nameof(ChangePasswordEndpoint))
        .WithSummary("Changes password")
        .WithDescription("Change password");
    }

}
