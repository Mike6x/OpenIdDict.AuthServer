using Identity.Infrastructure.Services.Clients.Endpoints;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using OpenIddict.Abstractions;

namespace Identity.Infrastructure.Services.CorsPolicy.Handlers;

public static class RefreshOrigins
{
    public static async Task<List<string>> Handler( 
                HttpContext httpContext,
                IConfiguration configuration,
                ICorsPolicyProvider corsPolicyProvider,
                IOpenIddictApplicationManager applicationManager,
                CancellationToken cancellationToken)
            {
                var defaultCorsPolicy = await corsPolicyProvider.GetPolicyAsync(httpContext, null) 
                                        ?? new ();
   
                defaultCorsPolicy.Origins.Clear();
                
                var allowedOrigins = configuration.GetSection("CorsOptions:Origins").Get<string[]>() ?? [];
                foreach (var origin in allowedOrigins)
                {
                    defaultCorsPolicy.Origins.Add(origin);
                }

                var listApplications = ApplicationHelper.ListApplicationsAsync(applicationManager);
                
                await foreach (var app in listApplications)
                {
                    var redirectUris = await applicationManager.GetRedirectUrisAsync(app, cancellationToken);
                    foreach (var uri in redirectUris.Select(s => new Uri(s)))
                    {
                        var origin = $"{uri.Scheme}://{uri.Authority}";
                        if (!defaultCorsPolicy.Origins.Contains(origin))
                        {
                            defaultCorsPolicy.Origins.Add(origin);
                        }
                    }
                }
                return defaultCorsPolicy.Origins.ToList();
            }
    
}