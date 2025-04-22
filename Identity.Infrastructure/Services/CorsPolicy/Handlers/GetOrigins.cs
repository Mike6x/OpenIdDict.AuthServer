using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace Identity.Infrastructure.Services.CorsPolicy.Endpoints;

public static class GetOrigins
{
    public static Task<List<string>>  Handler (IOptions<CorsOptions> corsOptions)
            {
                //var defaultCorsPolicy = await corsPolicyProvider.GetPolicyAsync(httpContext, null)
                
                var defaultCorsPolicy = corsOptions.Value.GetPolicy(corsOptions.Value.DefaultPolicyName)
                                        ?? new Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicy();
                
                return Task.FromResult(defaultCorsPolicy.Origins.ToList());
            }
}