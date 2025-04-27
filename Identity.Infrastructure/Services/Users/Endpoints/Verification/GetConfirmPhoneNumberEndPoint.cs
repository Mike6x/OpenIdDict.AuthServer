using Identity.Application.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services.Users.Endpoints.Verification
{

    public static class GetConfirmPhoneNumberEndpoint
    {
        internal static RouteHandlerBuilder MapGetCornfirmPhoneNumberEndpoint(this IEndpointRouteBuilder endpoints)
        {
            return endpoints.MapGet("/confirm-phone-number", (
                [FromQuery] string userId,
                [FromQuery] string code,
                IUserService userService,
                CancellationToken cancellationToken) =>
            {

                return Task.FromResult(userService.ConfirmPhoneNumberAsync(userId, code, cancellationToken));
            })
            .WithName(nameof(GetConfirmPhoneNumberEndpoint))
            .WithSummary("Confirm phone number")
            .WithDescription("Confirm phone number for a user.")
            .AllowAnonymous();
        }
        
    }
}
