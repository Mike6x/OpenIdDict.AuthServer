using FluentValidation;
using FluentValidation.Results;
using Identity.Application.Users.Abstractions;
using Identity.Application.Users.Features.EmailConfirm;
using Shared.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Users.Endpoints
{
    public static class ConirmEmailEndpoint
    {
        internal static RouteHandlerBuilder MapCornfirmEmailEndpoint(this IEndpointRouteBuilder endpoints)
        {
            return endpoints.MapPost("/confirm-email", async (
                EmailConfirmCommand command, 
                [FromHeader(Name = TenantConstants.Identifier)] string tenant, 
                [FromServices] IValidator<EmailConfirmCommand> validator, 
                IUserService userService, 
                CancellationToken cancellationToken) =>
            {
            
                ValidationResult result = await validator.ValidateAsync(command, cancellationToken);
                if (!result.IsValid)
                {
                    return Results.ValidationProblem(result.ToDictionary());
                }

                await userService.ConfirmEmailAsync(command.UserId, command.Code, command.Tenant, cancellationToken);
                
                return Results.Ok("Email Confirmed.");

            })
            .WithName(nameof(ConirmEmailEndpoint))
            .WithSummary("Confirm email")
            .WithDescription("Confirm email address for a user.")
            .AllowAnonymous();
        }
    }
}
