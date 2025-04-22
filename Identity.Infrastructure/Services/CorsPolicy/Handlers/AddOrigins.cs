using Identity.Application.CorsPolicy.Features;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;

namespace Identity.Infrastructure.Services.CorsPolicy.Handlers;

/// <summary>
/// Add specified origins to the list of allowed origins on default cors policy.
/// </summary>
/// <param name="origins"></param>
/// <returns></returns>
public static class AddOriginsEndpoint
{
    public static Task<List<string>>  Handler(AddOrRemoveOriginsCommand request, IOptions<CorsOptions> corsOptions)
    {
        var defaultCorsPolicy = corsOptions.Value.GetPolicy(corsOptions.Value.DefaultPolicyName)
                                ?? new Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicy();

        foreach (var uri in request.Origins.Select(s => new Uri(s)))
        {
            var origin = $"{uri.Scheme}://{uri.Authority}";
            if (!defaultCorsPolicy.Origins.Contains(origin))
            {
                defaultCorsPolicy.Origins.Add(origin);
            }
        }

        return Task.FromResult(defaultCorsPolicy.Origins.ToList());
    }
}