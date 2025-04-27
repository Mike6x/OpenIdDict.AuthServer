using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;

namespace Identity.Infrastructure.Services.CorsPolicy;

public static class OriginHelper
{
    /// <summary>
    /// Add a uri to list of allowed origins on default cors policy
    /// </summary>
    /// <param name="origins"></param>
    /// <param name="corsOptions"></param>
    /// <returns></returns>
    public static void AllowOriginsAsync(IEnumerable<Uri> origins, IOptions<CorsOptions> corsOptions)
    {
        var defaultCorsPolicy = corsOptions.Value.GetPolicy(corsOptions.Value.DefaultPolicyName)
            ?? new Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicy();
        
        foreach (var uri in origins)
        {
            var origin = $"{uri.Scheme}://{uri.Authority}";
            if (defaultCorsPolicy != null && !defaultCorsPolicy.Origins.Contains(origin))
            {
                defaultCorsPolicy.Origins.Add(origin);
            }
        }
        
    }
    
    /// <summary>
    /// Remmove a uri from list of allowed origins on default cors policy
    /// </summary>
    /// <param name="origins"></param>
    /// <returns></returns>
    public static void  RemoveOriginsAsync(IEnumerable<Uri> origins, IOptions<CorsOptions> corsOptions)
    {
        var defaultCorsPolicy = corsOptions.Value.GetPolicy(corsOptions.Value.DefaultPolicyName);
        foreach (var uri in origins)
        {
            var origin = $"{uri.Scheme}://{uri.Authority}";
            if (defaultCorsPolicy != null && defaultCorsPolicy.Origins.Contains(origin))
            {
                defaultCorsPolicy.Origins.Remove(origin);
            }
        }
    }
}


