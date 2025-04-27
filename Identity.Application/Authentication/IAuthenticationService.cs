using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;

namespace Identity.Application.Authentication;

public interface IAuthenticationService
{
    Task<IResult> LogInAsync(LoginRequest request);
    Task <IResult> LogOutAsync(string? returnUrl);
    
    Task<IResult> LogInCallBackAsync(HttpContext httpContext);
    
}