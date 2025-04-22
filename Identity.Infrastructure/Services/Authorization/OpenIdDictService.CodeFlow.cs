using Microsoft.AspNetCore.Http;
using OpenIddict.Server.AspNetCore;

namespace Identity.Infrastructure.Services.Authorization.Endpoints;

public partial class OpenIdDictService
{
    
    public IResult Deny()
    {
        return Results.Forbid(authenticationSchemes: new List<string>
        {
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
        });
    }

}